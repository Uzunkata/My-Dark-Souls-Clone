using System.Xml.Schema;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private MeleeWeaponDamageCollider meleeDamageCollider;

    public MeleeWeaponDamageCollider MeleeDamageCollider => meleeDamageCollider;

    private void Awake()
    {
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        meleeDamageCollider.Damage = weapon.Damage;
        meleeDamageCollider.CharacterCausingDMG = characterWieldingWeapon;

        meleeDamageCollider.LightAttackDamageModifier = weapon.LightAttackDamageModifier;
    }
}
