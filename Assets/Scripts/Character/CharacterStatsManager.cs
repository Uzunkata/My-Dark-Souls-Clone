using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterStatsManager : MonoBehaviour
{
    private CharacterManager character;

    [Header("StaminaRegeneration")]
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] private float staminaTickDelay = 0.1f;
    [SerializeField] private float staminaRegenerationDelay = 2;
    [SerializeField] private float staminaRegenerationAmount = 2;

    protected virtual void Awake() 
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        
    }
    public static int CalculateHealthBasedOnVitalityLevel(int vitality)
    {
        float health = 0;

        //TODO: create an equation for health calculation
        health = vitality * 12;

        return Mathf.RoundToInt(health);
    }

    public static int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;

        //TODO: create an equation for stamina calculation
        stamina = endurance * 8;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        //we want only owenrs to edit their respective network variables
        if (!character.IsOwner)
            return;

        //we don't want to regenerate stamina while sprinting
        if (character.IsSprinting)
            return;

        if (character.IsPerformingAction)
            return;

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.CurrentStamina.Value < character.MaxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;
                if (staminaTickTimer >= staminaTickDelay)
                {
                    staminaTickTimer = 0;
                    character.CurrentStamina.Value += staminaRegenerationAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        // we only want to reset the regeneration of the action used stamina
        // we don't want to reset the regeneration if we are already regenerating stamina
        if (currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }
}
