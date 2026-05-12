using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action/Light Attack Action")]
public class LightAttackWeaponItemAction :  WeaponItemAction
{

    [SerializeField] string animationName_MainHand = "Main_Light_Attack_01";
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
        if (performingPlayer.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_01, animationName_MainHand, true);
        }
        if (performingPlayer.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            //performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_OneHand_01, animationName_OffHand, true);
        }
    }
}
