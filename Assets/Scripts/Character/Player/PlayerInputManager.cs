using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    private static PlayerInputManager instance;
    private PlayerManager player;
    private PlayerControls playerControls;

    private readonly float walkingSpeedInputIndicator = 0.5f;
    private readonly float runningSpeedInputIndicator = 1f;

    public enum InputSystem
    {
        KEYBOARD,
        CONTROLLER
    }


    [Header("Player Movement Input")]
    [SerializeField] private InputSystem inputSystem = InputSystem.KEYBOARD;
    [SerializeField] private Vector2 playerMovementInput;
    [SerializeField] private float playerVerticalInput;
    [SerializeField] private float playerHorizontalInput;
    [SerializeField] private float moveAmount;

    [Header("Camera Movement Input")]
    [SerializeField] private Vector2 cameraMovementInput;
    [SerializeField] private float cameraVerticalInput;
    [SerializeField] private float cameraHorizontalInput;

    public static PlayerInputManager GetInstance
    {
        get { return instance; }
    }

    public float WalkingSpeedInputIndicator => walkingSpeedInputIndicator;
    public float RunningSpeedInputIndicator => runningSpeedInputIndicator;
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
        }
        //otherwise, we must be at the main menu => disable player controls
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable() 
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => playerMovementInput = i.ReadValue<Vector2>();
            playerControls.CameraMovement.Movement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
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
        HandlePlayerMovementInput();
        HandeCameraMovementInput();
    }

    private void HandlePlayerMovementInput()
    {
        playerVerticalInput = playerMovementInput.y;
        playerHorizontalInput = playerMovementInput.x;

        //we use absolute numbers because movement amount should aways be positive
        //we clamp the values because we want to work with 0, 0.5 and 1:
        //0 = still, 0.5 = walking, 1 = running
        moveAmount = Mathf.Clamp01(Mathf.Abs(playerVerticalInput) + Mathf.Abs(playerHorizontalInput));

        if (inputSystem == InputSystem.KEYBOARD && moveAmount > 0)
        {
            moveAmount -= 0.5f;
        }

        if (moveAmount > 0 && playerControls.PlayerMovement.Running.IsPressed())
        {
            moveAmount = runningSpeedInputIndicator;
        }
        else if (moveAmount > 0)
        {
            moveAmount = walkingSpeedInputIndicator;
        }

        // if (moveAmount <= walkingSpeedInputIndicator && moveAmount > 0)
        // {
        //     moveAmount = walkingSpeedInputIndicator;
        // }
        // else if (moveAmount > walkingSpeedInputIndicator && moveAmount <= runningSpeedInputIndicator)
        // {
        //     moveAmount = runningSpeedInputIndicator;
        // }

        if (player == null)
            return;

        //The reason we are apssing 0 to the horizontal movement
        //is because we want the player to move only where he is facing
        //when his camera is not locked on an object
        player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);

        //TODO: if we are locked on:
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

}
