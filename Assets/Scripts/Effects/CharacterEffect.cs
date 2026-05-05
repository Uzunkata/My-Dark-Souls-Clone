using UnityEngine;

public abstract class CharacterEffect : ScriptableObject
{

    [Header("Effect ID")]
    [SerializeField] private int effectID;

    public int EffectID
    {
        get => effectID;
        set => effectID = value;
    }
    
    public abstract void ProcessEffect(CharacterManager character);
}