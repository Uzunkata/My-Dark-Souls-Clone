using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    private static WorldSoundFXManager instance;

    public static WorldSoundFXManager GetInstance
    {
        get { return instance; }
    }

    [Header("Action Sound FX")]
    [SerializeField] private AudioClip rollSFX;

    [Header("Damage Sound FX")]
    [SerializeField] private AudioClip[] physicalDamageSFX;

    #region ENCAPSULATION

    public AudioClip RollSFX => rollSFX;
    public AudioClip[] PhysicalDamageSFX => physicalDamageSFX;

    #endregion

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

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        int randomIndex = Random.Range(0, array.Length - 1);
        return array[randomIndex];
    }
}
