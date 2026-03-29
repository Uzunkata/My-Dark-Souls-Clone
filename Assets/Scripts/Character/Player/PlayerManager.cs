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

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            //TODO: this will be moved when saving and loading is added
            playerNetworkManager.maxStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.Endurance);
            playerNetworkManager.currentStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.Endurance);

            PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.GetInstance.HandleAllCameraActions();
    }
}
