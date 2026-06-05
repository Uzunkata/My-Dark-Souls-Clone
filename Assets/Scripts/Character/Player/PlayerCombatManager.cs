using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerManager))]
public class PlayerCombatManager : CharacterCombatManager
{

    private PlayerManager player;

    [Header("Weapons")]
    [SerializeField] private WeaponItem weaponInUse;

    [Header("Flags")]
    [SerializeField] private bool canComboWithMainHandWeapon = false;
    [SerializeField] private bool canComboWithOffHandWeapon = false;


    #region ENCAPSULATION
    public WeaponItem WeaponInUse
    {
        get => weaponInUse;
        set => weaponInUse = value;
    }
    public bool CanComboWithMainHandWeapon
    {
        get => canComboWithMainHandWeapon;
        set => canComboWithMainHandWeapon = value;
    }
    public bool CanComboWithOffHandWeapon
    {
        get => canComboWithOffHandWeapon;
        set => canComboWithOffHandWeapon = value;
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
            case WeaponItemAction.AttackType.LightAttack_OneHand_01:
                staminaDeducted = weaponInUse.BaseStaminaCost * weaponInUse.LightAttackStaminaModifier;
                break;
            case WeaponItemAction.AttackType.HeavyAttack_OneHanded_01:
                staminaDeducted = weaponInUse.BaseStaminaCost * weaponInUse.HeavyAttackStaminaMulpiplier;
                break;
            default:
                break;
        }

        player.CurrentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player.IsOwner)
        {
            PlayerCamera.GetInstance.SetLockedCameraHeight();
        }
    }

    public override void EnableCanDoCombo()
    {
        if (player.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            canComboWithMainHandWeapon = true;
        }
        else if (player.PlayerNetworkManager.IsUsingOffHand.Value)
        {
            canComboWithOffHandWeapon = true;
        }
    }

    public override void DissableCanDoCombo()
    {
        canComboWithMainHandWeapon = false;
        canComboWithOffHandWeapon = false;
    }
}

