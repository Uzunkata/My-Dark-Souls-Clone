using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider damageCollider;

    [Header("Damage")]
    [SerializeField] protected Damage damage;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new();

    #region ENCAPSULATION

    public Damage Damage
    {
        get => damage;
        set => damage = value;
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            // TODO:
            // FRIENDLY FIRE?
            // CHECK IF TARGET IS BLOCKING
            // CHECK IF TARGET IS INVULNERABLE
            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        // MAKEING SURE WE DAMAGE TARGETS ONLY THE FIRST TIME
        if (charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);
        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.GetInstance.TakeDamageEffect);
        damageEffect.Damage = damage;
        damageEffect.ContactPoint = contactPoint;

        damageTarget.CharacterEffectsManager.ProcessEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear();
    }
}
