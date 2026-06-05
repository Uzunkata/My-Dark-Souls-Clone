using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action/Light Attack Action")]
public class LightAttackWeaponItemAction :  WeaponItemAction
{

    [SerializeField] public const string animationName_MainHand_01 = "Main_Light_Attack_01";
    [SerializeField] public const string animationName_MainHand_02 = "Main_Light_Attack_02";

    //[SerializeField] string animationName_OffHand = "Off_Light_Attack_01";


    public override void AttemptToPerformAction(PlayerManager performingPlayer, WeaponItem weapon)
    {
        base.AttemptToPerformAction(performingPlayer, weapon);

        if (!performingPlayer.IsOwner)
            return;

        if (performingPlayer.CurrentStamina.Value <= 0)
            return;

        // TODO: JUMP ATTACKS
        if (!performingPlayer.IsGrounded)
            return;


        PerformLightAttackAction(performingPlayer, weapon);
    }

    private void PerformLightAttackAction(PlayerManager performingPlayer, WeaponItem weapon)
    {
        if (performingPlayer.PlayerCombatManager.CanComboWithMainHandWeapon && performingPlayer.IsPerformingAction)
        {
            if (performingPlayer.CharacterCombatManager.LastAttackAnimationPerformed == animationName_MainHand_01)
            {
                performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_02, animationName_MainHand_02, true);
            }
            else
            {
                performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_01, animationName_MainHand_01, true);
            }
        }
        else if (!performingPlayer.IsPerformingAction)
        {
            performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_01, animationName_MainHand_01, true);
        }

        // if (performingPlayer.PlayerNetworkManager.IsUsingMainHand.Value)
        // {
        //     //performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_01, animationName_MainHand_01, true);
        // }
        // if (performingPlayer.PlayerNetworkManager.IsUsingOffHand.Value)
        // {
        //     //performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_01, animationName_OffHand, true);
        // }
    }
}
