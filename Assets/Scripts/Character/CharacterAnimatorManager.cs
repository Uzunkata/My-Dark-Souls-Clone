using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterManager))]
public class CharacterAnimatorManager : MonoBehaviour
{
    [HideInInspector] private CharacterManager character;

    public const float dampTime = 0.1f;
    public const float normalizedTransitionDuration = 0.2f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontal, float vertical)
    {
        character.Animator.SetFloat("Horizontal", horizontal, dampTime, Time.deltaTime);
        character.Animator.SetFloat("Vertical", vertical, dampTime, Time.deltaTime);
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
