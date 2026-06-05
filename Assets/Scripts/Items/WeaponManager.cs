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
        meleeDamageCollider.DamageModifiers = weapon.DamageModifiers;
        meleeDamageCollider.CharacterCausingDMG = characterWieldingWeapon;

        // meleeDamageCollider.LightAttack_01_DamageModifier = weapon.LightAttack_01_DamageModifier;
        // meleeDamageCollider.HeavyAttack_01_DamageModifier = weapon.HeavyAttack_01_DamageModifier;
        // meleeDamageCollider.ChargedHeavyAttack_01_DamageModifier = weapon.ChargedHeavyAttack_01_DamageModifier;
    }
}
