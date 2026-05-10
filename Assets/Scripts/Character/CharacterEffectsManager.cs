using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] private GameObject bloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessEffect(CharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        // if we have the manualy set VFX, use it
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        // otherwise use the default VFX
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.GetInstance.BloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
}
