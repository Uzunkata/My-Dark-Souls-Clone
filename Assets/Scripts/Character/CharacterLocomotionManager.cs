using System.Net.Security;
using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterLocomotionManager : MonoBehaviour
{

    CharacterManager character;

    [Header("Ground Chech & Jumpping")]
    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] protected LayerMask groundLyaer;
    [SerializeField] protected float groundCheckSphereRadius = 1;
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected float grounded_Y_Velocity = -20;
    [SerializeField] protected float fallStart_Y_Velocity = -5;
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {  
        HandleGroundCheck();
        HandleGravity();
    }
    
    protected void HandleGroundCheck()
    {
        character.IsGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLyaer);
        character.Animator.SetBool("IsGrounded", character.IsGrounded);
    }

    protected virtual void HandleGravity()
    {
        if (character.IsGrounded)
        {
            // DONT DO THIS IF WE ARE JUMPING (MOVING UP)
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = grounded_Y_Velocity;
            }
        }
        else
        {
            if (!character.IsJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStart_Y_Velocity;
            }

            inAirTimer += Time.deltaTime;
            yVelocity.y += gravityForce * Time.deltaTime;
            character.Animator.SetFloat("InAirTimer", inAirTimer);
        }

        if (character.IsOwner)
            character.Move(yVelocity * Time.deltaTime);

    }

    // DEBUG OUR GROUND CHECK SPHERE (DELETE LATER)
    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }
}
