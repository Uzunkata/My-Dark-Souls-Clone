using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : MonoBehaviour
{
    [Header("Stat Bars")]
    [SerializeField] private UI_StatBar healthBar;
    [SerializeField] private UI_StatBar staminaBar;

    [Header("Quick Item Slots")]
    [SerializeField] private Image mainWeaponQuickSlotIcon;
    [SerializeField] private Image offWeaponQuickSlotIcon;

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetNewHealthValue(int oldValue, int newValue)
    {
        healthBar.SetStat(newValue);
    }
    public void SetMaxHealthValue(int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }
    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(newValue);
    }
    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void SetMainWeaponQuickSlotIcon(int weaponID)
    {
        SetWeaponQuickSlotIcon(weaponID, ref mainWeaponQuickSlotIcon);
    }
    public void SetOffWeaponQuickSlotIcon(int weaponID)
    {
        SetWeaponQuickSlotIcon(weaponID, ref offWeaponQuickSlotIcon);
    }
    private void SetWeaponQuickSlotIcon(int weaponID, ref Image quickSlotIcon)
    {
        WeaponItem weapon = WorldItemDatabase.GetInstance.GetWeaponByID(weaponID);
        if (weapon == null)
        {
            quickSlotIcon.enabled = false;
            quickSlotIcon.sprite = null;
            return;
        }
        
        if (weapon.ItemIcon == null)
        {
            Debug.Log("MAIN HAND WEAPON HAS NO ICON: " + weaponID);
            quickSlotIcon.enabled = false;
            quickSlotIcon.sprite = null;
            return;
        }

        // TODO: IF WE DO NOT MEET WEAPON REQUIREMENTS -> DRAW AN X IN THE ICON

        quickSlotIcon.sprite = weapon.ItemIcon;
        quickSlotIcon.enabled = true;
    }
}
