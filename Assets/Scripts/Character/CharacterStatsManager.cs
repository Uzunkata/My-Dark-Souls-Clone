using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterStatsManager : MonoBehaviour
{
    private CharacterManager character;

    [Header("StaminaRegeneration")]
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] private float staminaRegenerationDelay = 2;
    [SerializeField] private float staminaRegenerationAmount = 2;

    protected virtual void Awake() 
    {
        character = GetComponent<CharacterManager>();
    }

    public static int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;

        // create an equation for stamina calculation
        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        //we want only owenrs to edit their network variables
        if (!character.IsOwner)
            return;

        //we dont want to renerate stamina while sprinting
        if (character.CharacterNetworkManager.IsSprinting)
            return;

        if (character.IsPerformingAction)
            return;

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.CharacterNetworkManager.currentStamina.Value < character.CharacterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.CharacterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
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
