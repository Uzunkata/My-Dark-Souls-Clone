using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    [SerializeField] private CharacterManager characterCausingDMG;

    [Header("Weapon Attack Modifiers")]
    [SerializeField] private float lightAttackDamageModifier;

    public CharacterManager CharacterCausingDMG
    {
        get => characterCausingDMG;
        set => characterCausingDMG = value;
    }
    public float LightAttackDamageModifier
    {
        get => lightAttackDamageModifier;
        set => lightAttackDamageModifier = value;
    }

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
            case WeaponItemAction.AttackType.LightAttack01:
                ApplyAttackDamageModifiers(lightAttackDamageModifier, damageEffect);
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
