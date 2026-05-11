using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    private static WorldUtilityManager instance;
    public static WorldUtilityManager GetInstance
    {
        get { return instance; }
    }

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

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

    public LayerMask CharacterLayers => characterLayers;
    public LayerMask EnvironmentLayers => environmentLayers;

}
