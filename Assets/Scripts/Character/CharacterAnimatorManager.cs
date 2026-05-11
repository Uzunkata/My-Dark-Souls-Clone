using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterManager))]
public class CharacterAnimatorManager : MonoBehaviour
{
    [HideInInspector] private CharacterManager character;

    private int horizontalHash;
    private int verticalHash;

    [Header("Damage Animations")]
    [SerializeField] private string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
    [SerializeField] private string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
    [SerializeField] private string hit_Left_Medium_01 = "Hit_Left_Medium_01";
    [SerializeField] private string hit_Right_Medium_01 = "Hit_Right_Medium_01";

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
        float snappedHorizontalAmount = RoundMovement(horizontalMovement);
        float snappedVerticalAmount = RoundMovement(verticalMovement);

        if (isSprinting)
        {
            snappedVerticalAmount = animationSprintIndicator;
        }

        character.Animator.SetFloat(horizontalHash, snappedHorizontalAmount, dampTime, Time.deltaTime);
        character.Animator.SetFloat(verticalHash, snappedVerticalAmount, dampTime, Time.deltaTime);
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

    public virtual void PlayTargetAttackActionAnimation(
        WeaponItemAction.AttackType attackType,
        string targetAnimation,
         bool isAction, 
         bool applyRootMotion = true, 
         bool canRotate = false, 
         bool canMove = false)
    {
        // TODO: 
        // KEEP TRACK OF WAST ATTACK PERFORMED (FOR COMBOS)
        // KEEP TRACK OF CURRENT ATTACK TYPE (PARRY, BLOCK, ...)
        // UPADTE ANIMATION SET TO CURRENT WEAPON ANIMATION
        // TELL THE NETWORK WE ARE IN ATTACKING FLAG

        character.CharacterCombatManager.CurrentAttackType = attackType;
        character.ApplyRootMotion = applyRootMotion;
        character.Animator.CrossFade(targetAnimation, normalizedTransitionDuration);
        character.IsPerformingAction = isAction;
        character.CanMove = canMove;
        character.CanRotate = canRotate;


        character.CharacterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public string GetDirectionalDamageAnimation(float angleHitFrom)
    {
        if (angleHitFrom >= 145 && angleHitFrom <= 180)
        {
            return hit_Forward_Medium_01;
        } 
        else if (angleHitFrom <= -145 && angleHitFrom >= -180)
        {
            return hit_Forward_Medium_01;
        }
        else if (angleHitFrom >= -45 && angleHitFrom <= 45)
        {
            return hit_Backward_Medium_01;
        }
        else if (angleHitFrom >= -144 && angleHitFrom <= -45)
        {
            return hit_Left_Medium_01;
        }
        else if (angleHitFrom >= 45 && angleHitFrom <= 144)
        {
            return hit_Right_Medium_01;
        }

        throw new System.Exception("such an angle does not exist????");
    }

    private float RoundMovement(float movement)
    {
        float result = 0;

        if (movement > 0 && movement <= 0.5f)
            result = 0.5f;
        if (movement > 0.5f && movement <= 1f)
            result = 1;
        if (movement < 0 && movement >= -0.5f)
            result = -0.5f;
        if (movement < -0.5f && movement >= -1)
            result = -1;

        return result;
    }
}
