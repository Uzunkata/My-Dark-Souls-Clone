using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    [HideInInspector] private float verticalMovement;
    [HideInInspector] private float horizontalMovement;
    [HideInInspector] private float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float sprintingSpeed = 6.5f;
    [SerializeField] private float sprintingStaminaCost = 2;
    [SerializeField] private float rotationSpeed = 15;
    
    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] private float dodgeStaminaCost = 25;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.CharacterNetworkManager.VerticalMovement = verticalMovement;
            player.CharacterNetworkManager.HorizontalMovement = horizontalMovement;
            player.CharacterNetworkManager.MoveAmount = moveAmount;
        }
        else
        {
            verticalMovement = player.CharacterNetworkManager.VerticalMovement;
            horizontalMovement = player.CharacterNetworkManager.VerticalMovement;
            moveAmount = player.CharacterNetworkManager.MoveAmount;

            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.PlayerNetworkManager.IsSprinting);
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundMovement();
        HandleRotation();
        //TODO: JUMPING MOVEMENT
        //TODO: FALLING
    }

    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.GetInstance.PlayerVerticalInput;
        horizontalMovement = PlayerInputManager.GetInstance.PlayerHorizontalInput;
        moveAmount = PlayerInputManager.GetInstance.MoveAmount;

        //Clamp the movements
    }

    private void HandleGroundMovement()
    {
        if (!player.CanMove)
            return;

        GetVerticalAndHorizontalInputs();
        //our movement direction is based on camera facing perspective and our inputs (WASD)
        moveDirection = PlayerCamera.GetInstance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.GetInstance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        //we don't want to move up and right.
        moveDirection.y = 0;

        if (player.PlayerNetworkManager.IsSprinting)
        {
            player.Move(sprintingSpeed * Time.deltaTime * moveDirection);
        }
        else
        { 
            if (PlayerInputManager.GetInstance.MoveAmount > PlayerInputManager.WALKING_INPUT_INDICATOR)
            {
                player.Move(runningSpeed * Time.deltaTime * moveDirection);
            } 
            else if (PlayerInputManager.GetInstance.MoveAmount >= PlayerInputManager.RUNNING_INPUT_INDICATOR)
            {
                player.Move(walkingSpeed * Time.deltaTime * moveDirection);
            }
        }

        // if (PlayerInputManager.GetInstance.GetMovementSpeedType == PlayerInputManager.MovementSpeedType.RUNNING)
        // {
        //     player.Move(moveDirection * runningSpeed * Time.deltaTime);
        // } 
        // else if (PlayerInputManager.GetInstance.GetMovementSpeedType == PlayerInputManager.MovementSpeedType.WALKING)
        // {
        //     player.Move(moveDirection * walkingSpeed * Time.deltaTime);
        // }
    }

    private void HandleRotation()
    {
        if (!player.CanRotate)
            return;

       targetRotationDirection = Vector3.zero;
       targetRotationDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * verticalMovement;
       targetRotationDirection = targetRotationDirection + PlayerCamera.GetInstance.GetCameraObject().transform.right * horizontalMovement;
       targetRotationDirection.Normalize();
       targetRotationDirection.y = 0;

       if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge()
    {
        if (player.IsPerformingAction)
            return;

        if (player.PlayerNetworkManager.currentStamina.Value <= 0)
            return;

        //if we are moving, performe a roll
        if (moveAmount > 0)
        {
            rollDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * verticalMovement;
            //rollDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * PlayerInputManager.GetInstance.PlayerVerticalInput;
            rollDirection += PlayerCamera.GetInstance.GetCameraObject().transform.right * horizontalMovement;
            //rollDirection += PlayerCamera.GetInstance.GetCameraObject().transform.right * PlayerInputManager.GetInstance.PlayerHorizontalInput;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.PlayerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true);
        }
        //if we are not moving, performe a backstep
        else
        {
            player.PlayerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true);
        }

        player.PlayerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }

    public void HandleSprinting()
    {
        if (player.IsPerformingAction)
        {
            player.PlayerNetworkManager.IsSprinting = false;
        }

        if (player.PlayerNetworkManager.currentStamina.Value <= 0)
        {
            player.PlayerNetworkManager.IsSprinting = false;
            return;
        }

        //we can sprint only if we are moving
        if (moveAmount > 0)
        {
            player.PlayerNetworkManager.IsSprinting = true;
        }
        else
        {
            player.PlayerNetworkManager.IsSprinting = false;
        }

        if (player.PlayerNetworkManager.IsSprinting)
        {
            player.PlayerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }
}
