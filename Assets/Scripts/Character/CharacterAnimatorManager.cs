using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;

[RequireComponent(typeof(CharacterManager))]
public class CharacterAnimatorManager : MonoBehaviour
{
    [HideInInspector] private CharacterManager character;

    private int horizontalHash;
    private int verticalHash;

    public const float dampTime = 0.1f;
    public const float normalizedTransitionDuration = 0.2f;
    public const float animationSprintIndicator = 2;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isSprinting)
        {
            verticalAmount = animationSprintIndicator;
        }

        character.Animator.SetFloat(horizontalHash, horizontalAmount, dampTime, Time.deltaTime);
        character.Animator.SetFloat(verticalHash, verticalAmount, dampTime, Time.deltaTime);
    }

    //
    public virtual void PlayTargetActionAnimation(
        string targetAnimation,
         bool isAction, 
         bool applyRootMotion = true, 
         bool canRotate = false, 
         bool canMove = false)
    {
        character.ApplyRootMotion = applyRootMotion;
        character.Animator.CrossFade(targetAnimation, normalizedTransitionDuration);
        character.IsPerformingAction = isAction;
        character.CanMove = canMove;
        character.CanRotate = canRotate;

        // tell the server/hosts we played an animation, 
        //and play that animation for everyone present
        character.CharacterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

}
