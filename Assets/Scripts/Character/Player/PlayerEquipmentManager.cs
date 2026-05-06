using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerEquipmentManager : CharacterEquipmentManager
{
    private PlayerManager player;
    [SerializeField] private WeaponModelInstantiationSlot rightHandSlot;
    [SerializeField] private WeaponModelInstantiationSlot leftHandSlot;

    [SerializeField] private WeaponManager rightWeaponManager;
    [SerializeField] private WeaponManager leftWeaponManager;

    [SerializeField] private GameObject rightHandWeaponModel;
    [SerializeField] private GameObject leftHandWeaponModel;

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
                case WeaponItem.WeaponModelSlot.LeftHand: 
                    leftHandSlot = weaponSlot;
                    break;
                case WeaponItem.WeaponModelSlot.RightHand: 
                    rightHandSlot = weaponSlot;
                    break;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadLeftWeapon();
        LoadRightWeapon();
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
            case WeaponItem.WeaponModelSlot.RightHand:
                player.PlayerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, true, true, true);
                break;
            case WeaponItem.WeaponModelSlot.LeftHand:
                player.PlayerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, true, true, true);
                break;
            default:
                return;
        }
        player.PlayerNetworkManager.SetWeaponID(weapon.ItemID, hand);
    }

    // RIGHT WEAPON
    public void LoadRightWeapon()
    {
        WeaponItem rightHandWeapon = player.PlayerInventoryManager.CurrentRHWeapon;
        LoadWeapon(player, rightHandWeapon, ref rightHandWeaponModel, ref rightHandSlot, ref rightWeaponManager);
    }

    public void SwitchRightWeapon()
    {
        SwitchWeapon(WeaponItem.WeaponModelSlot.RightHand);
    }

    // LEFT WEAPON
    public void LoadLeftWeapon()
    {
        WeaponItem leftHandWeapon = player.PlayerInventoryManager.CurrentLHWeapon;
        LoadWeapon(player, leftHandWeapon, ref leftHandWeaponModel, ref leftHandSlot, ref leftWeaponManager);
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

    public void SwitchLeftWeapon()
    {
        SwitchWeapon(WeaponItem.WeaponModelSlot.LeftHand);
    }
    
}
