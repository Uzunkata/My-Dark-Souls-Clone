using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{

    [SerializeField] private float staminaDamage;

    public float StaminaDamage
    {
        get => staminaDamage;
        set => staminaDamage = value;
    }

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);
        
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        // COMPARE THE STAMINA DAMAGE AGAINST OTHER EFFECTS BEFORE

        if (character.IsOwner)
        {
            character.CurrentStamina.Value -= StaminaDamage;
        }
    }
}
