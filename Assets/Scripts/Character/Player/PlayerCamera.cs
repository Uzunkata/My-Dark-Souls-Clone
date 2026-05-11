using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Singleton Patern
    private static PlayerCamera instance;

    public static PlayerCamera GetInstance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private Camera cameraObject;
    private PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;        // transform that thakes care of the Up & Down movement and position

    // change these to tweak camera performance
    [Header("Camera Settings")]
    //the bigger this number is, the longer it will be 
    //for the camera to reach its position during movement
    [SerializeField] private float cameraFollowSmoothSpeed = 0;
    [SerializeField] private float upAndDownRotationSpeed = 220;
    [SerializeField] private float leftAndRightRotationSpeed = 300;
    [SerializeField] private float minimumPivot = -30;      // the lowlest point you are abler to look down
    [SerializeField] private float maximumPivot = 60;       // the heighes point you are abler to look up
    [SerializeField] private float cameraColisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayers;

    // just displays camera values
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;       // values used for camera collision: moves the camera object to this position upon coloding
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    private float cameraZPosition;        // values used for camera collision
    private float targetCammeraZPosition;        // values used for camera collision

    [Header("Camera Lock On")]
    [SerializeField] private float lockOnRadius = 20;
    [SerializeField] private float minimumViewableAngle = -50;
    [SerializeField] private float maximumViewableAngle = 50;
    [SerializeField] private float cameraLockOnHeightSpeed = 1;
    [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] private float unlockedCameraHeight = 1.65f;
    [SerializeField] private float lockedCameraHeight = 2;

    private List<CharacterManager> availableTargets = new();
    private CharacterManager nearersLockOnTarget;
    private CharacterManager leftLockOnTarget;
    private CharacterManager rightLockOnTarget;
    private Coroutine cameraLockOnHeightCoroutine;

    public void SetPlayer(PlayerManager player) { this.player = player; }
    public CharacterManager NearersLockOnTarget => nearersLockOnTarget;
    public CharacterManager LeftLockOnTarget => leftLockOnTarget;
    public CharacterManager RightLockOnTarget => rightLockOnTarget;
    public PlayerManager GetPlayer() { return this.player; }
    public Camera GetCameraObject() { return cameraObject; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        // we use this to move our cammera when coliding (we move along z axis)
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition 
            = Vector3.SmoothDamp(
                transform.position, 
                player.transform.position, 
                ref cameraVelocity, 
                cameraFollowSmoothSpeed + Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        if (player.PlayerNetworkManager.IsLockedOn.Value)
        {
            // THIS ROTATES THIS GAME OBJECT
            Vector3 rotationDirection = player.PlayerCombatManager.CurrentLockedOnTarget.CharacterCombatManager.LockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // THIS ROTATES THE PIVOT OBJECT
            rotationDirection = player.PlayerCombatManager.CurrentLockedOnTarget.CharacterCombatManager.LockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();
            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // SAVE OUR ROTATION TO OUR LOOK ANGLES, SO WHEN WE UNLOCK IT IT DOESN'T SNAP TOO FAR AWAY
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        else
        {
            // normal rotations
            // roate left and right based on the horizontal movement of the mouse
            leftAndRightLookAngle += PlayerInputManager.GetInstance.CameraHorizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
            // roate up and down based on the vertical movement of the mouse
            upAndDownLookAngle -= PlayerInputManager.GetInstance.CameraVerticalInput * upAndDownRotationSpeed * Time.deltaTime;
            // clamp the up and down look angle between the min and a max value
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            // rotate this gameobject left and right
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // rotate this gameobject up and down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }

    private void HandleCollisions()
    {
        targetCammeraZPosition = cameraZPosition;
        RaycastHit hit;
        // direction to check for object collision
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // we check if there is an object infront of our desired direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraColisionRadius, direction, out hit, Mathf.Abs(targetCammeraZPosition), collideWithLayers))
        {
            // if there is, we get the camera distance form it
            float distanceFromObject = Vector3.Distance(cameraObject.transform.position, hit.point);  //or also: float distanceFromObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            // we then equate our target z position to the following
            targetCammeraZPosition = -(distanceFromObject - cameraColisionRadius);
        }

        // if our target position is less than our collision radius, we substract our collision radius (making it snap back)
        if (Mathf.Abs(targetCammeraZPosition) < cameraColisionRadius)
        {
            targetCammeraZPosition = -cameraColisionRadius;
        }

        // we then apply our final position using a lerp over a time of 0.2f
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCammeraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
        }

    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; // WHEN YOU MOVE LEFT, X GETS NEGATIVE

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.GetInstance.CharacterLayers);
                                                                            
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget != null)
            {
                // CHECH IF WITHIN FIELD OF VIEW
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                if (lockOnTarget == player)
                    continue;

                if (lockOnTarget.IsDead.Value)
                    continue;

                // check to see if the target is blocked (for example by a wall)
                if (viewableAngle < minimumViewableAngle || viewableAngle > maximumViewableAngle)
                    continue;
                
                RaycastHit hit;
                if (Physics.Linecast(
                    player.PlayerCombatManager.LockOnTransform.position, 
                    lockOnTarget.CharacterCombatManager.LockOnTransform.position, 
                    out hit, 
                    WorldUtilityManager.GetInstance.EnvironmentLayers))
                {
                    // we hit something => we can't see our target
                    continue;
                }
                else
                {
                    availableTargets.Add(lockOnTarget);
                }
            }
        }

        foreach (CharacterManager character in availableTargets)
        {
            if (character != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, character.transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearersLockOnTarget = character;
                }

                if (player.PlayerNetworkManager.IsLockedOn.Value)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(character.transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.x;

                    if (character == player.PlayerCombatManager.CurrentLockedOnTarget)
                        continue;

                    if (relativeEnemyPosition.x <= 0.00f && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = character;
                    }
                    else if(relativeEnemyPosition.x >= 0.00f && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = character;
                    }
                }
            }
            else
            {
                ClearLockOnTargets();
                player.PlayerNetworkManager.IsLockedOn.Value = false;
            }
        }
    }

    public void SetLockedCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null)
            StopCoroutine(cameraLockOnHeightCoroutine);

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                if (player.PlayerCombatManager.CurrentLockedOnTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, cameraLockOnHeightSpeed);

                    cameraPivotTransform.transform.localRotation = 
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = 
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, cameraLockOnHeightSpeed);
                }
            }

            yield return null;
        }

        // FAILSAFE: if for some reason the camera does not go back to the intended height
        // we will set it outside the while loop, causing it to snap => we will know if something is wrong
        if (player != null)
        {
            if (player.PlayerCombatManager.CurrentLockedOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }

    public void ClearLockOnTargets()
    {
        nearersLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.IsPerformingAction)
        {
            yield return null;
        }

        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (player.PlayerCombatManager.CurrentLockedOnTarget == null && nearersLockOnTarget != null)
        {
            player.PlayerCombatManager.SetTarget(nearersLockOnTarget);
            player.PlayerNetworkManager.IsLockedOn.Value = true;
        }

        yield return null;
    }
}
