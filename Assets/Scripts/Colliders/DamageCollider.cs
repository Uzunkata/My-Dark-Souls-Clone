using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] protected Damage damage;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new();

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponent<CharacterManager>();

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
}
