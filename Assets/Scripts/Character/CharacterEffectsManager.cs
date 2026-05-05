using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessEffect(CharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }
}
