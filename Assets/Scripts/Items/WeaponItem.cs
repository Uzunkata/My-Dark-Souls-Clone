using UnityEngine;

public class WeaponItem : Item
{

    // TODO:
    // ANIMATION CONTROLER
    // WEAPON MODIFIERS
    // WEAPON DEFLECTION WHEN ATTACKING SHIELDS
    // ITEM BASED ACTIONS
    // ASH OF WAR
    // BLOCKING SOUND

    [Header("Weapon Model")]
    [SerializeField] private GameObject weaponModel;

    [Header("Weapon Requirements")]
    [SerializeField] private int strREQ = 0;
    [SerializeField] private int dexREQ = 0;
    [SerializeField] private int intREQ = 0;
    [SerializeField] private int faithREQ = 0;

    [Header("Weapon Base Damage")]
    [SerializeField] private Damage damage;

    [Header("Attack Modifier")]
    [SerializeField] private float lightAttack_01_DamageModifier = 1.1f;
    [SerializeField] private float heavyAttack_01_DamageModifier = 1.5f;
    [SerializeField] private float chargedHeavyAttack_01_DamageModifier = 2f;


    [Header("Weapon Poise Damage")]
    [SerializeField] private float poiseDamage = 10;

    [Header("Weapon Stamina Cost")]
    [SerializeField] private int baseStaminaCost = 20;
    [SerializeField] private float lightAttackStaminaMulpiplier = 0.9f;
    [SerializeField] private float heavyAttackStaminaMulpiplier = 1.2f;

    [Header("Weapon Actions")]
    [SerializeField] private WeaponItemAction lightAttack_OneHanded;
    [SerializeField] private WeaponItemAction heavyAttack_OneHanded;

    // TODO:
    // RUNNING/HEAVY ATTACK STAMINA COST

    public enum WeaponModelSlot
    {
        MainHand,
        OffHand,
        // TODO:
        // RIGHT/LEFT HIP
        // BACK
        // ...
    }

    #region ENCAPSULATION

    public GameObject WeaponModel
    {
        get => weaponModel;
        set => weaponModel = value;
    }
    public Damage Damage
    {
        get => damage;
        set => damage = value;
    }
    public int StrREQ
    {
        get => strREQ;
        set => strREQ = value;
    }
    public int DexREQ
    {
        get => dexREQ;
        set => dexREQ = value;
    }
    public int IntREQ
    {
        get => intREQ;
        set => intREQ = value;
    }
    public int FaithREQ
    {
        get => faithREQ;
        set => faithREQ = value;
    }
    public int BaseStaminaCost
    {
        get => baseStaminaCost;
        set => baseStaminaCost = value;
    }
    public float LightAttackStaminaModifier => lightAttackStaminaMulpiplier;
    public float HeavyAttackStaminaMulpiplier => heavyAttackStaminaMulpiplier;

    public float PoiseDamage
    {
        get => poiseDamage;
        set => poiseDamage = value;
    }
    public WeaponItemAction LightAttack_OneHanded => lightAttack_OneHanded;
    public WeaponItemAction HeavyAttack_OneHanded => heavyAttack_OneHanded;
    public float LightAttack_01_DamageModifier => lightAttack_01_DamageModifier;
    public float HeavyAttack_01_DamageModifier => heavyAttack_01_DamageModifier;
    public float ChargedHeavyAttack_01_DamageModifier => chargedHeavyAttack_01_DamageModifier;

    #endregion
}
