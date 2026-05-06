using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        meleeDamageCollider.Damage = weapon.Damage;
        meleeDamageCollider.CharacterCausingDMG = characterWieldingWeapon;
    }
}
