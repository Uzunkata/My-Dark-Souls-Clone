using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerManager))]
public class PlayerCombatManager : CharacterCombatManager
{

    private PlayerManager player;
    [SerializeField] private WeaponItem weaponInUse;

    #region ENCAPSULATION
    public WeaponItem WeaponInUse
    {
        get => weaponInUse;
        set => weaponInUse = value;
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponAcion(WeaponItem weapon, WeaponItemAction weaponAction)
    {
        if (player.IsOwner)
        {
            // PERFOREM THE ACTION
            weaponAction.AttemptToPerformAction(player, weapon);

            // PERFORME THE ACTION FOR OTHER PLAYERS
            player.PlayerNetworkManager.NotifyTheServerOfWeaponActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.ActionID, weapon.ItemID);
        }
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        if (!player.IsOwner)
            return;
        
        if (weaponInUse == null)
            return;

        float staminaDeducted = 0;

        switch(CurrentAttackType)
        {
            case WeaponItemAction.AttackType.LightAttack01:
                staminaDeducted = weaponInUse.BaseStaminaCost * weaponInUse.LightAttackStaminaModifier;
                break;
            default:
                break;
        }

        player.CurrentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }
}

