using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    protected virtual private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.GetInstance.rollSFX);
    }
}
