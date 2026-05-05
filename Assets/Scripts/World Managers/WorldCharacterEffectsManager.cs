using UnityEngine;
using System.Collections.Generic;


public class WorldCharacterEffectsManager : MonoBehaviour
{
    private static WorldCharacterEffectsManager instance;
    public static WorldCharacterEffectsManager GetInstance
    {
        get { return instance; }
    }

    [Header("Damage")]
    [SerializeField] private TakeHealthDamageEffect takeDamageEffect;
    [SerializeField] private List<CharacterEffect> characterEffects; 

    public TakeHealthDamageEffect TakeDamageEffect => takeDamageEffect;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            onAwake();
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    private void onAwake()
    {
        GenerateEffectIDs();
    }
    
    private void GenerateEffectIDs()
    {
        for ( int i = 0; i < characterEffects.Count; i++)
        {
            characterEffects[i].EffectID = i;
        }
    }

    
}
