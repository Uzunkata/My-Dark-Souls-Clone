using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action/Light Attack Action")]
public class LightAttackWeaponItemAction :  WeaponItemAction
{

    [SerializeField] string animationName_Right = "Right_Light_Attack_01";
    [SerializeField] string animationName_Left = "Left_Light_Attack_01";
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
        if (performingPlayer.PlayerNetworkManager.IsUsingRightHand.Value)
        {
            performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, animationName_Right, true);
        }
        if (performingPlayer.PlayerNetworkManager.IsUsingRightHand.Value)
        {
            //performingPlayer.PlayerAnimatorManager.PlayTargetAttackActionAnimation(animationName_Left, true);
        }
    }
}
