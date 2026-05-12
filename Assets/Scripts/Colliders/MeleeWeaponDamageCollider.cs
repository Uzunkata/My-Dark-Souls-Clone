using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    [SerializeField] private CharacterManager characterCausingDMG;

    [Header("Weapon Attack Modifiers")]
    [SerializeField] private float lightAttack_01_DamageModifier;
    [SerializeField] private float heavyAttack_01_DamageModifier;
    [SerializeField] private float chargedHeavyAttack_01_DamageModifier;

    #region ENCAPSULATION

    public CharacterManager CharacterCausingDMG
    {
        get => characterCausingDMG;
        set => characterCausingDMG = value;
    }
    public float LightAttack_01_DamageModifier
    {
        get => lightAttack_01_DamageModifier;
        set => lightAttack_01_DamageModifier = value;
    }
    public float HeavyAttack_01_DamageModifier
    {
        get => heavyAttack_01_DamageModifier;
        set => heavyAttack_01_DamageModifier = value;
    }
    public float ChargedHeavyAttack_01_DamageModifier
    {
        get => chargedHeavyAttack_01_DamageModifier;
        set => chargedHeavyAttack_01_DamageModifier = value;
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponentInChildren<Collider>();
        damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            if (damageTarget == characterCausingDMG)
                return;

            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            // TODO:
            // FRIENDLY FIRE?
            // CHECK IF TARGET IS BLOCKING
            // CHECK IF TARGET IS INVULNERABLE
            DamageTarget(damageTarget);
        }
    }

    protected override void DamageTarget(CharacterManager damagedTarget)
    {
        // MAKEING SURE WE DAMAGE TARGETS ONLY THE FIRST TIME
        if (charactersDamaged.Contains(damagedTarget))
            return;

        charactersDamaged.Add(damagedTarget);
        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.GetInstance.TakeDamageEffect);
        damageEffect.Damage = damage;
        damageEffect.ContactPoint = contactPoint;
        damageEffect.AngleHitFrom = Vector3.SignedAngle(characterCausingDMG.transform.forward, damagedTarget.transform.forward, Vector3.up);

        switch(characterCausingDMG.CharacterCombatManager.CurrentAttackType)
        {
            case WeaponItemAction.AttackType.LightAttack_OneHand_01:
                ApplyAttackDamageModifiers(lightAttack_01_DamageModifier, damageEffect);
                break;
            case WeaponItemAction.AttackType.HeavyAttack_OneHanded_01:
                ApplyAttackDamageModifiers(heavyAttack_01_DamageModifier, damageEffect);
                break;
                case WeaponItemAction.AttackType.ChargedHeavyAttack_OneHanded_01:
                ApplyAttackDamageModifiers(chargedHeavyAttack_01_DamageModifier, damageEffect);
                break;
            default:
                break;
        }

        //damageTarget.CharacterEffectsManager.ProcessEffect(damageEffect);

        // only owners know their damage coliders and send the request once throught the network
        if (characterCausingDMG.IsOwner)
        {   
            Debug.Log("I have delt: " + damageEffect.Damage.CalculateFinalDamage());

            damagedTarget.CharacterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damagedTarget.NetworkObjectId, 
                characterCausingDMG.NetworkObjectId, 
                damageEffect.Damage,
                damageEffect.AngleHitFrom,
                damageEffect.ContactPoint.x,
                damageEffect.ContactPoint.y,
                damageEffect.ContactPoint.z);
        }
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeHealthDamageEffect damageEffect)
    {
        damageEffect.Damage = damageEffect.Damage.GetModifiedDamage(modifier);

        // TODO: 
        // IF ATTACK IS FULLY CHARGED HEAVY, MULTIPLY BY FULL CHARGE MODIFIER
        // AFTER NORMAL MODIFIERHAVE BEEN CALCULATED
    }
}
