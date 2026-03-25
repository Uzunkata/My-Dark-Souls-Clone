using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    private float verticalMovement;
    private float horizontalMovement;
    private float moveAmount;

    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [SerializeField]
    private float walkingSpeed = 2;
    [SerializeField]
    private float runningSpeed = 5;
    [SerializeField]
    private float rotationSpeed = 15;
    

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
            player.CharacterNetworkManager.SetVerticalMovement(verticalMovement);
            player.CharacterNetworkManager.SetHorizontalMovement(horizontalMovement);
            player.CharacterNetworkManager.SetMoveAmount(moveAmount);
        }
        else
        {
            verticalMovement = player.CharacterNetworkManager.GetVerticalMovement();
            horizontalMovement = player.CharacterNetworkManager.GetVerticalMovement();
            moveAmount = player.CharacterNetworkManager.GetMoveAmount();

            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
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
        GetVerticalAndHorizontalInputs();
        //our movement direction is based on camera facing perspective and our inputs (WASD)
        moveDirection = PlayerCamera.GetInstance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.GetInstance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        //we don't want to move up and right.
        moveDirection.y = 0;

        if (PlayerInputManager.GetInstance.MoveAmount > PlayerInputManager.GetInstance.WalkingSpeedInputIndicator)
        {
            player.Move(moveDirection * runningSpeed * Time.deltaTime);
        } 
        else if (PlayerInputManager.GetInstance.MoveAmount >= PlayerInputManager.GetInstance.WalkingSpeedInputIndicator)
        {
            player.Move(moveDirection * walkingSpeed * Time.deltaTime);
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

}
