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
    [SerializeField] LayerMask collideWithLayers;

    // just displays camera values
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;       // values used for camera collision: moves the camera object to this position upon coloding
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    private float cameraZPosition;        // values used for camera collision
    private float targetCammeraZPosition;        // values used for camera collision

    public void SetPlayer(PlayerManager player) { this.player = player; }
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
        // TODO: if we are locked on, force rotation towards target

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
}
