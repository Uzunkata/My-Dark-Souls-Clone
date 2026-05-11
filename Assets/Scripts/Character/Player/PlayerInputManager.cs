using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    private static PlayerInputManager instance;
    private PlayerManager player;
    private PlayerControls playerControls;

   public const float WALKING_INPUT_INDICATOR = 0.5f;
   public const float RUNNING_INPUT_INDICATOR = 1f;

    // public enum InputSystem
    // {
    //     KEYBOARD,
    //     CONTROLLER
    // }

    [Header("Camera Movement Input")]
    [SerializeField] private Vector2 cameraMovementInput;
    [SerializeField] private float cameraVerticalInput;
    [SerializeField] private float cameraHorizontalInput;
    [SerializeField] private bool lockOnInput = false;
    //[SerializeField] private bool seekLockOnTargetInput = false;
    [SerializeField] private bool seekLeftLockOnInput = false;
    [SerializeField] private bool seekRightLockOnInput = false;
    private Coroutine lockOnCoroutine;


    [Header("Player Movement Input")]
    // [SerializeField] private InputSystem inputSystem = InputSystem.KEYBOARD;
    [SerializeField] private Vector2 playerMovementInput;
    [SerializeField] private float playerVerticalInput;
    [SerializeField] private float playerHorizontalInput;
    [SerializeField] private float moveAmount;

    [Header("Player Action Input")]
    [SerializeField] private bool dodgeInput = false;
    [SerializeField] private bool sprintInput = false;
    [SerializeField] private bool jumpInput = false;
    [SerializeField] private bool rightMouseInput = false;
    [SerializeField] private bool leftMouseInput = false;

    public static PlayerInputManager GetInstance
    {
        get { return instance; }
    }
    public float PlayerVerticalInput => playerVerticalInput;
    public float PlayerHorizontalInput => playerHorizontalInput;
    public float MoveAmount => moveAmount;
    public float CameraVerticalInput => cameraVerticalInput;
    public float CameraHorizontalInput => cameraHorizontalInput;

    public void SetPlayer(PlayerManager player) { this.player = player; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            //we want only one instance of this script at one time, if another exists, destroy it
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        //when the scene changes, run the logic
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
        if (playerControls != null)
            playerControls.Disable();
    }

    //the reason we need to disable this script before enetring the world, is because
    //the network manager spawns the player charachter when the game boots =>
    //we would move the character when in the menue if inputs are enabled
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        //if we are loading into our world scene, enable the player controls
        if (newScene.buildIndex == WorldSaveGameManager.GetInstance.GetWorldSceneIndex())
        {
            instance.enabled = true;
                    
            if (playerControls != null)
                playerControls.Enable();
        }
        //otherwise, we must be at the main menu => disable player controls
        else
        {
            instance.enabled = false;

            if (playerControls != null)
                playerControls.Disable();
        }
    }

    private void OnEnable() 
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => playerMovementInput = i.ReadValue<Vector2>();
            playerControls.CameraMovement.Movement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.RightMouse.performed += i => rightMouseInput = true;
            playerControls.PlayerActions.LeftMouse.performed += i => leftMouseInput = true;
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;

            playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => seekLeftLockOnInput = true;
            playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => seekRightLockOnInput = true;
            //playerControls.PlayerActions.SeekLockOnTarget.performed += i => seekLockOnTargetInput = true;
            //playerControls.PlayerActions.SeekLockOnTarget.canceled += i => seekLockOnTargetInput = false;

        }

        playerControls.Enable();
    }

    void OnDestroy()
    {
        // if we destroy this object, unsubscribe from the event 
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void Update()
    {
        //we want this to be updated every single frame we are using the keyboard
        HandleAllInputs();
    }

    private void HandleAllInputs()
    {
        // MOVEMENT:
        HandeCameraMovementInput();
        HandlePlayerMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();

        // WEAPON ACTIONS
        HandleRightMouseInput();
        HandleLeftMouseInput();

        //  CAMERA LOCK IN
        HandleLockOnInput();
        HandleLeftLockOnTargetSwitchInput();
        HandleRightLockOnTargetSwitchInput();
    }

    //  MOVEMENT
    private void HandlePlayerMovementInput()
    {
        playerVerticalInput = playerMovementInput.y;
        playerHorizontalInput = playerMovementInput.x;

        //we use absolute numbers because movement amount should aways be positive
        //we clamp the values because we want to work with 0, 0.5 and 1:
        //0 = still, 0.5 = walking, 1 = running
        moveAmount = Mathf.Clamp01(Mathf.Abs(playerVerticalInput) + Mathf.Abs(playerHorizontalInput));

        if (moveAmount <= WALKING_INPUT_INDICATOR && moveAmount > 0)
        {
            moveAmount = WALKING_INPUT_INDICATOR;
        }
        else if (moveAmount > WALKING_INPUT_INDICATOR && moveAmount <= RUNNING_INPUT_INDICATOR)
        {
            moveAmount = RUNNING_INPUT_INDICATOR;
        }

        if (player == null)
            return;

        if (!player.PlayerNetworkManager.IsLockedOn.Value || player.PlayerNetworkManager.IsSprinting)
        {
            //The reason we are apssing 0 to the horizontal movement
            //is because we want the player to move only where he is facing
            //when his camera is not locked on an object
            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.IsSprinting);  
        }
        else
        {
            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(playerHorizontalInput, playerVerticalInput, player.IsSprinting);  
        }
    }

    private void HandeCameraMovementInput()
    {
        cameraVerticalInput = cameraMovementInput.y;
        cameraHorizontalInput = cameraMovementInput.x;
    }

    //if we minimize the window => stop reading inputs
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    //  ACTIONS
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            //TODO: cant doge when a menu or ui windows is open
            player.PlayerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.PlayerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.IsSprinting = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            // TODO: DISSABLE IF WE HAVE UI OPEN

            player.PlayerLocomotionManager.AttemptToPerformJump();
        }
    }

    private void HandleRightMouseInput()
    {
        if (rightMouseInput)
        {
            rightMouseInput = false;

            // TODO: DO NOT PERFORME IF WE HAVE UI OPEN

            //player.PlayerNetworkManager.SetCharacterActionHand(true);

            // TODO: TWOHANDING

            if (player.PlayerInventoryManager.CurrentRHWeapon != null)
            {
                player.PlayerNetworkManager.SetCharacterActionHand(true);
                player.PlayerCombatManager.PerformWeaponAcion(player.PlayerInventoryManager.CurrentRHWeapon, player.PlayerInventoryManager.CurrentRHWeapon.LightAttack_OneHanded);
            }
        }
    }

    private void HandleLeftMouseInput()
    {
        if (leftMouseInput)
        {
            leftMouseInput = false;

            // TODO: DO NOT PERFORME IF WE HAVE UI OPEN

            //player.PlayerNetworkManager.SetCharacterActionHand(false);

            // TODO: TWOHANDING

            if (player.PlayerInventoryManager.CurrentLHWeapon != null)
            {
                player.PlayerNetworkManager.SetCharacterActionHand(false);
                player.PlayerCombatManager.PerformWeaponAcion(player.PlayerInventoryManager.CurrentLHWeapon, player.PlayerInventoryManager.CurrentLHWeapon.LightAttack_OneHanded);
            }
        }
    }

    private void HandleLockOnInput()
    {
        if (player.PlayerNetworkManager.IsLockedOn.Value)
        {
            if (player.PlayerCombatManager.CurrentLockedOnTarget == null)
                return;

            if(player.PlayerCombatManager.CurrentLockedOnTarget.IsDead.Value)
            {
                player.PlayerNetworkManager.IsLockedOn.Value = false;
                // FIND NEW TARGET?
            }

            // this assures us that the coroutine never runs multiple times, overlapping itself
            if (lockOnCoroutine != null)
                StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = StartCoroutine(PlayerCamera.GetInstance.WaitThenFindNewTarget());
        }

        if (lockOnInput)
        {
            lockOnInput = false;
            if (player.PlayerNetworkManager.IsLockedOn.Value)
            {
                player.PlayerNetworkManager.IsLockedOn.Value = false;
                PlayerCamera.GetInstance.ClearLockOnTargets();
            }
            else
            {
                // TODO: IF WE ARE AIMING/USING A RANGE WEAPON => DON'T ENABLE?

                PlayerCamera.GetInstance.HandleLocatingLockOnTargets();

                if (PlayerCamera.GetInstance.NearersLockOnTarget != null)
                {
                    player.PlayerCombatManager.SetTarget(PlayerCamera.GetInstance.NearersLockOnTarget);
                    player.PlayerNetworkManager.IsLockedOn.Value = true;
                }
            }
        }
    }

    private void HandleLeftLockOnTargetSwitchInput()
    {
        if (seekLeftLockOnInput)
        {
            seekLeftLockOnInput = false;

            if (player.PlayerNetworkManager.IsLockedOn.Value)
            {
                PlayerCamera.GetInstance.HandleLocatingLockOnTargets();

                if (PlayerCamera.GetInstance.LeftLockOnTarget != null)
                {
                    player.PlayerCombatManager.SetTarget(PlayerCamera.GetInstance.LeftLockOnTarget);
                }
            }
        }
    }

    private void HandleRightLockOnTargetSwitchInput()
    {
        if (seekRightLockOnInput)
        {
            seekRightLockOnInput = false;

            if (player.PlayerNetworkManager.IsLockedOn.Value)
            {
                PlayerCamera.GetInstance.HandleLocatingLockOnTargets();

                if (PlayerCamera.GetInstance.RightLockOnTarget != null)
                {
                    player.PlayerCombatManager.SetTarget(PlayerCamera.GetInstance.RightLockOnTarget);
                }
            }
        }
    }
}
