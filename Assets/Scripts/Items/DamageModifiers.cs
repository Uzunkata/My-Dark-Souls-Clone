using UnityEngine;

[System.Serializable]
public class DamageModifiers
{
    [SerializeField] private float lightAttack_01_DamageModifier = 1.0f;
    [SerializeField] private float lightAttack_02_DamageModifier = 1.2f;
    [SerializeField] private float heavyAttack_01_DamageModifier = 1.4f;
    [SerializeField] private float heavyAttack_02_DamageModifier = 1.6f;
    [SerializeField] private float chargedHeavyAttack_01_DamageModifier = 2.0f;
    [SerializeField] private float chargedHeavyAttack_02_DamageModifier = 2.2f;

    public float LightAttack_01_DamageModifier => lightAttack_01_DamageModifier;
    public float LightAttack_02_DamageModifier => lightAttack_02_DamageModifier;
    public float HeavyAttack_01_DamageModifier => heavyAttack_01_DamageModifier;
    public float HeavyAttack_02_DamageModifier => heavyAttack_02_DamageModifier;
    public float ChargedHeavyAttack_01_DamageModifier => chargedHeavyAttack_01_DamageModifier;
    public float ChargedHeavyAttack_02_DamageModifier => chargedHeavyAttack_02_DamageModifier;
}
