using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public static int WEAPON_SLOTS_PER_HAND = 3;

    [SerializeField] private WeaponItem currentRightHandWeapon;
    [SerializeField] private WeaponItem currentLeftHandWeapon;

    [Header("Quick Slots")]
    [SerializeField] private WeaponItem[] weaponsInRightHandSlots = new WeaponItem[WEAPON_SLOTS_PER_HAND];
    [SerializeField] private WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[WEAPON_SLOTS_PER_HAND];
    //  -1 means that there is no weapon equiped
    [SerializeField] private int currentRHWeaponIndex = -1;
    [SerializeField] private int currentLHWeaponIndex = -1;

    #region ENCAPSULATION
    public WeaponItem CurrentRHWeapon
    {
        set => currentRightHandWeapon = value;
        get => currentRightHandWeapon;
    }
    public WeaponItem CurrentLHWeapon
    {
        set => currentLeftHandWeapon = value;
        get => currentLeftHandWeapon;
    }
    public WeaponItem[] WeaponsInRightHandSlots
    {
        set => weaponsInRightHandSlots = value;
        get => weaponsInRightHandSlots;
    }
    public WeaponItem[] WeaponsInLeftHandSlots
    {
        set => weaponsInLeftHandSlots = value;
        get => weaponsInLeftHandSlots;
    }

    #endregion

    public void ScrowEquipedWeapons(WeaponItem.WeaponModelSlot hand, bool increase = true)
    {
        switch (hand)
        {
            case WeaponItem.WeaponModelSlot.LeftHand:
                MoveIndex(increase, ref currentLHWeaponIndex);
                break;
            case WeaponItem.WeaponModelSlot.RightHand:
                MoveIndex(increase, ref currentRHWeaponIndex);
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
            case WeaponItem.WeaponModelSlot.LeftHand:
                index = currentLHWeaponIndex;
                break;
            case WeaponItem.WeaponModelSlot.RightHand:
                index = currentRHWeaponIndex;
                break;
            default:
                return null;
        }

        if (index == -1)
            return WorldItemDatabase.GetInstance.UnarmedWeapon;

        switch(hand)
        {
            case WeaponItem.WeaponModelSlot.LeftHand:
                result = weaponsInLeftHandSlots[index];
                break;
            case WeaponItem.WeaponModelSlot.RightHand:
                result = weaponsInRightHandSlots[index];
                break;
        }

        return result;
    }

    public WeaponItem UnequipHand(WeaponItem.WeaponModelSlot hand)
    {
        switch(hand)
        {
            case WeaponItem.WeaponModelSlot.LeftHand:
                currentLHWeaponIndex = -1;
                break;
            case WeaponItem.WeaponModelSlot.RightHand:
                currentRHWeaponIndex = -1;
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
            WeaponItem.WeaponModelSlot.LeftHand => currentLHWeaponIndex,
            WeaponItem.WeaponModelSlot.RightHand => currentRHWeaponIndex,
            _ => -200,
        };
    }

}
