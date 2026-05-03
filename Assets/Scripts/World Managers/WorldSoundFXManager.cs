using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    private static WorldSoundFXManager instance;

    public static WorldSoundFXManager GetInstance
    {
        get { return instance; }
    }

    [Header("Action Sound FX")]
    public AudioClip rollSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }
}
