using System;
using System.Collections;
using Unity.VisualScripting;
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

    [Header("Jump")]
    [SerializeField] private float jumpStaminaCost = 25;
    [SerializeField] private float jumpHeight = 2;
    private Vector3 jumpDirection;
    [SerializeField] private float jumpForwardSpeed = 5;
    [SerializeField] private float freeFallingSpeed = 2;

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
            horizontalMovement = player.CharacterNetworkManager.HorizontalMovement;
            moveAmount = player.CharacterNetworkManager.MoveAmount;

            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.IsSprinting);
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
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
        moveDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * verticalMovement;
        moveDirection += PlayerCamera.GetInstance.GetCameraObject().transform.right * horizontalMovement;
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

    private void HandleJumpingMovement()
    {
        // WE DONT CHECK FOR OWNER BECAUSE PLAYER CONTROLS ARE ASSIGNED AND PERFORMED PER PLAYER
        // AND THEY NEVER CONFLICT WITH EACH OTHER (ops caps)

        if (player.IsJumping)
        {
            player.Move(jumpForwardSpeed * Time.deltaTime * jumpDirection);
        }
    }

    private void HandleRotation()
    {
        if (!player.CanRotate)
            return;

        GetVerticalAndHorizontalInputs();
       targetRotationDirection = Vector3.zero;
       targetRotationDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * verticalMovement;
       targetRotationDirection += PlayerCamera.GetInstance.GetCameraObject().transform.right * horizontalMovement;
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

    private void HandleFreeFallMovement()
    {
        // if (!player.IsOwner)
        //     Debug.Log("I control my flight!!!");

        if (!player.IsGrounded)
        {
            GetVerticalAndHorizontalInputs();
            Vector3 freeFallDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * verticalMovement;
            freeFallDirection += PlayerCamera.GetInstance.GetCameraObject().transform.right * horizontalMovement;
            freeFallDirection.y = 0;

            player.Move(Time.deltaTime * freeFallingSpeed * freeFallDirection);
        }
    }

    public void AttemptToPerformDodge()
    {
        if (player.IsPerformingAction || player.IsJumping)
            return;

        if (player.CurrentStamina.Value <= 0)
            return;

        //if we are moving, performe a roll
        if (moveAmount > 0)
        {
            GetVerticalAndHorizontalInputs();
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

        player.CurrentStamina.Value -= dodgeStaminaCost;
    }

    public void AttemptToPerformJump()
    {
        // TODO: JUMPING ATTACK (TWO HANDING)
        if (player.IsPerformingAction)
            return;

        if (player.CurrentStamina.Value <= 0)
            return;

        if (player.IsJumping)
            return;

        if (!player.IsGrounded)
            return;
        

        //player.PlayerAnimatorManager.PlayTargetActionAnimation("Main_Jump_Start_01", false, false, true);
        player.PlayerAnimatorManager.PlayTargetActionAnimation("Main_Jump_Start_01", false, true, true);
        player.IsJumping = true;

        player.CurrentStamina.Value -= jumpStaminaCost;

        GetVerticalAndHorizontalInputs();
        jumpDirection = PlayerCamera.GetInstance.GetCameraObject().transform.forward * verticalMovement;
        jumpDirection += PlayerCamera.GetInstance.GetCameraObject().transform.right * horizontalMovement;
        jumpDirection.y = 0;
        //rollDirection.Normalize();

        if (jumpDirection != Vector3.zero)
        {  
            if (player.IsSprinting)
            {
                jumpDirection *= 1;
            }
            else if (PlayerInputManager.GetInstance.MoveAmount > PlayerInputManager.WALKING_INPUT_INDICATOR)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.GetInstance.MoveAmount <= PlayerInputManager.WALKING_INPUT_INDICATOR)
            {
                jumpDirection *= 0.25f;
            }
        }

        //Debug.Log("I control my flight: " + jumpDirection);
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    public void HandleSprinting()
    {
        if (player.IsPerformingAction)
        {
            player.IsSprinting = false;
        }

        if (player.CurrentStamina.Value <= 0)
        {
            player.IsSprinting = false;
            return;
        }

        //we can sprint only if we are moving
        if (moveAmount > 0)
        {
            player.IsSprinting = true;
        }
        else
        {
            player.IsSprinting = false;
        }

        if (player.IsSprinting)
        {
            player.CurrentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }
}
