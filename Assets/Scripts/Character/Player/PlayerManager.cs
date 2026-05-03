using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerLocomotionManager))]
[RequireComponent(typeof(PlayerAnimatorManager))]
[RequireComponent(typeof(PlayerNetworkManager))]
[RequireComponent(typeof(PlayerStatsManager))]
public class PlayerManager : CharacterManager
{
    [HideInInspector] private PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] private PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] private PlayerNetworkManager playerNetworkManager;
    [HideInInspector] private PlayerStatsManager playerStatsManager;

    public PlayerLocomotionManager PlayerLocomotionManager => playerLocomotionManager;
    public PlayerAnimatorManager PlayerAnimatorManager => playerAnimatorManager;
    public PlayerNetworkManager PlayerNetworkManager => playerNetworkManager;
    public PlayerStatsManager PlayerStatsManager => playerStatsManager;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        //if we are not the owner of this objedct (network owner),
        //then let it's owner deal with it's update
        if (!IsOwner)
            return;

        //here we put all the things we want to handle only when we are in the game world
        if (SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.GetInstance.GetWorldSceneIndex())
        {
            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.GetInstance.SetPlayer(this);
            PlayerInputManager.GetInstance.SetPlayer(this);
            WorldSaveGameManager.GetInstance.SetPlayer(this);

            playerNetworkManager.CurrentStamina.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.CurrentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            //TODO: this will be moved when saving and loading is added
            float stamina = CharacterStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.Endurance);
            playerNetworkManager.MaxStamina.Value = stamina;
            playerNetworkManager.CurrentStamina.Value = stamina;
            PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxStaminaValue(stamina);
        }
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.GetInstance.HandleAllCameraActions();
    }

    public CharacterSaveData GetCharacterSaveData()
    {
        CharacterSaveData characterSaveData = new CharacterSaveData()
        {
            CharacterName = "GOSU",

            XPosition = transform.position.x,
            YPosition = transform.position.y,
            ZPosition = transform.position.z,

            CurrentStamina = playerNetworkManager.CurrentStamina.Value,
        };

        return characterSaveData;
    }

    public void LoadCharacterSaveData(CharacterSaveData characterSaveData)
    {
        playerNetworkManager.CharacterName = characterSaveData.CharacterName;
        Vector3 playerPosition = new(characterSaveData.XPosition, characterSaveData.YPosition, characterSaveData.ZPosition);
        transform.position = playerPosition;

        playerNetworkManager.CurrentStamina.Value = characterSaveData.CurrentStamina;
    }
}
