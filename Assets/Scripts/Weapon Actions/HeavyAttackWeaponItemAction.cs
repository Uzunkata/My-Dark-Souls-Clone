using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction :  WeaponItemAction
{

    [SerializeField] string animationName_MainHand = "Main_Heavy_Attack_01";
    //[SerializeField] string animationName_OffHand = "Off_Heavy_Attack_01";
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


        PerformHeavyAttackAction(performingPlayer, weapon);
    }

    private void PerformHeavyAttackAction(PlayerManager performingPlayer, WeaponItem weapon)
    {
        if (performingPlayer.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack_OneHanded_01, animationName_MainHand, true);
        }
        if (performingPlayer.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            //performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack_OneHanded_01, animationName_OffHand, true);
        }
    }
}
