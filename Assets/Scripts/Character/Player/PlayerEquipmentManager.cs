using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerEquipmentManager : CharacterEquipmentManager
{
    private PlayerManager player;
    [SerializeField] private WeaponModelInstantiationSlot mainHandSlot;
    [SerializeField] private WeaponModelInstantiationSlot offHandSlot;

    [SerializeField] private WeaponManager mainHandWeaponManager;
    [SerializeField] private WeaponManager offHandWeaponManager;

    [SerializeField] private GameObject mainHandWeaponModel;
    [SerializeField] private GameObject offHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InitializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach(var weaponSlot in weaponSlots)
        {
            switch (weaponSlot.WeaponSlot)
            {
                case WeaponItem.WeaponModelSlot.OffHand: 
                    offHandSlot = weaponSlot;
                    break;
                case WeaponItem.WeaponModelSlot.MainHand: 
                    mainHandSlot = weaponSlot;
                    break;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadOffHandWeapon();
        LoadMainHandWeapon();
    }

    private void SwitchWeapon(WeaponItem.WeaponModelSlot hand)
    {
        if (!player.IsOwner)
            return;

        if (player.PlayerInventoryManager.GetHandIndex(hand) == PlayerInventoryManager.WEAPON_SLOTS_PER_HAND - 1)
        {
            EquipWeapon(player.PlayerInventoryManager.UnequipHand(hand), hand);
            return;
        }

        while(true)
        {
            player.PlayerInventoryManager.ScrowEquipedWeapons(hand);
            WeaponItem scrowedWeapon = player.PlayerInventoryManager.GetWeaponFromHand(hand);

            if (scrowedWeapon.ItemID != WorldItemDatabase.GetInstance.UnarmedWeapon.ItemID)
            {
                EquipWeapon(scrowedWeapon, hand);
                return;
            }

            if (player.PlayerInventoryManager.GetHandIndex(hand) == PlayerInventoryManager.WEAPON_SLOTS_PER_HAND - 1)
            {
                EquipWeapon(player.PlayerInventoryManager.UnequipHand(hand), hand);
                return;
            }
        }
    }

    private void EquipWeapon(WeaponItem weapon, WeaponItem.WeaponModelSlot hand)
    {
        if (!player.IsOwner)
            return;

        switch (hand)
        {
            case WeaponItem.WeaponModelSlot.MainHand:
                player.PlayerAnimatorManager.PlayTargetActionAnimation("Swap_MainHand_Weapon_01", true, false, true, true);
                break;
            case WeaponItem.WeaponModelSlot.OffHand:
                player.PlayerAnimatorManager.PlayTargetActionAnimation("Swap_OffHand_Weapon_01", true, false, true, true);
                break;
            default:
                return;
        }
        player.PlayerNetworkManager.SetWeaponID(weapon.ItemID, hand);
    }

    private void LoadWeapon(PlayerManager player, WeaponItem weapon, ref GameObject weaponModel, ref WeaponModelInstantiationSlot handSlot, ref WeaponManager weaponManager)
    {
        if (weapon != null)
        {
            weaponModel = Instantiate(weapon.WeaponModel);
            handSlot.LoadWeapon(weaponModel);
            weaponManager = weaponModel.GetComponent<WeaponManager>();
            weaponManager.SetWeaponDamage(player, weapon);
        }
    }
    // RIGHT WEAPON
    public void LoadMainHandWeapon()
    {
        WeaponItem mainHandWeapon = player.PlayerInventoryManager.CurrentMHWeapon;
        LoadWeapon(player, mainHandWeapon, ref mainHandWeaponModel, ref mainHandSlot, ref mainHandWeaponManager);
    }

    public void SwitchMainHandWeapon()
    {
        SwitchWeapon(WeaponItem.WeaponModelSlot.MainHand);
    }

    // LEFT WEAPON
    public void LoadOffHandWeapon()
    {
        WeaponItem offHandWeapon = player.PlayerInventoryManager.CurrentOHWeapon;
        LoadWeapon(player, offHandWeapon, ref offHandWeaponModel, ref offHandSlot, ref offHandWeaponManager);
    }

    public void SwitchOffHandWeapon()
    {
        SwitchWeapon(WeaponItem.WeaponModelSlot.OffHand);
    }
    
    // DAMAGE COLLIDERS
    public void EnableDamageColliders()
    {
        // TODO: IF WE ARE DUAL WIELDING WE WILL ENABLE BOTH

        if (player.PlayerNetworkManager.IsUsingOffHand.Value)
        {
            offHandWeaponManager.MeleeDamageCollider.EnableDamageCollider();
        }

        if (player.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            mainHandWeaponManager.MeleeDamageCollider.EnableDamageCollider();
        }

        // TODO: PLAY WEAPON SWING SOUND FX
    }

    public void DisableDamageColliders()
    {
        if (player.PlayerNetworkManager.IsUsingOffHand.Value)
        {
            offHandWeaponManager.MeleeDamageCollider.DisableDamageCollider();
        }

        if (player.PlayerNetworkManager.IsUsingMainHand.Value)
        {
            mainHandWeaponManager.MeleeDamageCollider.DisableDamageCollider();
        }
    }
}
