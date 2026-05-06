using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    [SerializeField] private CharacterManager characterCausingDMG;

    public CharacterManager CharacterCausingDMG
    {
        get => characterCausingDMG;
        set => characterCausingDMG = value;
    }
}
