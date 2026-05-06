using UnityEngine;

public class WeaponModelInstantiationSlot : MonoBehaviour
{
    // TODO:
    // WHAT SLOT IS THIS?

    [SerializeField] private GameObject currentWeaponModel;
    [SerializeField] private WeaponItem.WeaponModelSlot weaponSlot;

    public GameObject CurrentWeaponModel
    {
        get => currentWeaponModel;
        set => currentWeaponModel = value;
    }

    public WeaponItem.WeaponModelSlot WeaponSlot
    {
        get => weaponSlot;
        set => weaponSlot = value;
    }

    public void UnloadWeapon()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeapon(GameObject weaponModel)
    {
        UnloadWeapon();
        //currentWeaponModel = Instantiate(weaponModel);
        currentWeaponModel = weaponModel;
        currentWeaponModel.transform.parent = transform;

        currentWeaponModel.transform.localPosition = Vector3.zero;
        currentWeaponModel.transform.localRotation = Quaternion.identity;
        currentWeaponModel.transform.localScale = Vector3.one;
    }
}
