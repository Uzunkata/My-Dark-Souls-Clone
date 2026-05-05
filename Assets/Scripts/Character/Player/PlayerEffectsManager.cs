using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("Debug Delete Later")]
    [SerializeField] CharacterEffect effectToTest;
    [SerializeField] bool processTestEffect = false;

    private void Update()
    {
        if (processTestEffect)
        {
            processTestEffect = false;
            // WITH "Instantiate" WE CAN CHANGE AND TEST THE VALUE OF OUR EFFECT
            // WITTHOUT IT CHANGING THE VALUE ON THE ORIGINAL EFFECT

            //CharacterEffect effect = Instantiate(effectToTest);
            
            TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
            effect.StaminaDamage = 55;

            ProcessEffect(effect);
        }
    }
}
