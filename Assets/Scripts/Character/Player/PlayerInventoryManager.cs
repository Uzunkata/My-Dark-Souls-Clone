using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public static int WEAPON_SLOTS_PER_HAND = 3;

    [SerializeField] private WeaponItem currentMainHandWeapon;
    [SerializeField] private WeaponItem currentOffHandWeapon;

    [Header("Quick Slots")]
    [SerializeField] private WeaponItem[] weaponsInMainHandSlots = new WeaponItem[WEAPON_SLOTS_PER_HAND];
    [SerializeField] private WeaponItem[] weaponsInOffHandSlots = new WeaponItem[WEAPON_SLOTS_PER_HAND];
    //  -1 means that there is no weapon equiped
    [SerializeField] private int currentMHWeaponIndex = -1;
    [SerializeField] private int currentOHWeaponIndex = -1;

    #region ENCAPSULATION
    public WeaponItem CurrentMHWeapon
    {
        set => currentMainHandWeapon = value;
        get => currentMainHandWeapon;
    }
    public WeaponItem CurrentOHWeapon
    {
        set => currentOffHandWeapon = value;
        get => currentOffHandWeapon;
    }
    public WeaponItem[] WeaponsInRightHandSlots
    {
        set => weaponsInMainHandSlots = value;
        get => weaponsInMainHandSlots;
    }
    public WeaponItem[] WeaponsInLeftHandSlots
    {
        set => weaponsInOffHandSlots = value;
        get => weaponsInOffHandSlots;
    }

    #endregion

    public void ScrowEquipedWeapons(WeaponItem.WeaponModelSlot hand, bool increase = true)
    {
        switch (hand)
        {
            case WeaponItem.WeaponModelSlot.OffHand:
                MoveIndex(increase, ref currentOHWeaponIndex);
                break;
            case WeaponItem.WeaponModelSlot.MainHand:
                MoveIndex(increase, ref currentMHWeaponIndex);
                break;
            default:
                return;
        }
    }

    private void MoveIndex(bool increase, ref int index)
    {
        if (increase)
            index++;
        else
            index--;

        if (index == WEAPON_SLOTS_PER_HAND)
            index = 0;
    }

    public WeaponItem GetWeaponFromHand(WeaponItem.WeaponModelSlot hand)
    {
        int index;
        WeaponItem result = null;

        switch(hand)
        {
            case WeaponItem.WeaponModelSlot.OffHand:
                index = currentOHWeaponIndex;
                break;
            case WeaponItem.WeaponModelSlot.MainHand:
                index = currentMHWeaponIndex;
                break;
            default:
                return null;
        }

        if (index == -1)
            return WorldItemDatabase.GetInstance.UnarmedWeapon;

        switch(hand)
        {
            case WeaponItem.WeaponModelSlot.OffHand:
                result = weaponsInOffHandSlots[index];
                break;
            case WeaponItem.WeaponModelSlot.MainHand:
                result = weaponsInMainHandSlots[index];
                break;
        }

        return result;
    }

    public WeaponItem UnequipHand(WeaponItem.WeaponModelSlot hand)
    {
        switch(hand)
        {
            case WeaponItem.WeaponModelSlot.OffHand:
                currentOHWeaponIndex = -1;
                break;
            case WeaponItem.WeaponModelSlot.MainHand:
                currentMHWeaponIndex = -1;
                break;
            default:
                return null;
        }

        return WorldItemDatabase.GetInstance.UnarmedWeapon;
    }

    public int GetHandIndex(WeaponItem.WeaponModelSlot hand)
    {
        return hand switch
        {
            WeaponItem.WeaponModelSlot.OffHand => currentOHWeaponIndex,
            WeaponItem.WeaponModelSlot.MainHand => currentMHWeaponIndex,
            _ => -200,
        };
    }

}
