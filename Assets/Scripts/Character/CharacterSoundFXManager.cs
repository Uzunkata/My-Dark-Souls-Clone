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
        PlaySoundFX(WorldSoundFXManager.GetInstance.RollSFX);
        //audioSource.PlayOneShot(WorldSoundFXManager.GetInstance.RollSFX);
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        
        audioSource.PlayOneShot(soundFX, volume);
        // reset our pitch
        audioSource.pitch = 1;

        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }
}
