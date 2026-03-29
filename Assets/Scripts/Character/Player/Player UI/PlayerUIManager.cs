using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    private static PlayerUIManager instance;
    public static PlayerUIManager GetInstance
    {
        get { return instance; }
    }

    [HideInInspector] private PlayerUIHudManager playerUIHudManager;

    public PlayerUIHudManager PlayerUIHudManager => playerUIHudManager;
    
        //for DEBUGGING
    [Header("NETWORK JOIN")]
    [SerializeField]
    private bool startGameAsClient;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            //we want only one instance of this script at one time, if another exists, destroy it
            Destroy(gameObject);
        }

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            //we must first shutdown the network as a host,
            //in order to connect as a client
            NetworkManager.Singleton.Shutdown();

            //we start the network as a client
            NetworkManager.Singleton.StartClient();
        }
    }
}
