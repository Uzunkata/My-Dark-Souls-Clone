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

    [Header("Weapon Poise Damage")]
    [SerializeField] private float poiseDamage = 10;

    [Header("Weapon Stamina Cost")]
    [SerializeField] private int baseStaminaCost = 0;
    // TODO:
    // RUNNING/LIGHT/HEAVY ATTACK STAMINA COST

    public enum WeaponModelSlot
    {
        RightHand,
        LeftHand,
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
    public float PoiseDamage
    {
        get => poiseDamage;
        set => poiseDamage = value;
    }

    #endregion
}
