using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Collections;
using Unity.Netcode;
using System;

[RequireComponent(typeof(PlayerLocomotionManager))]
[RequireComponent(typeof(PlayerAnimatorManager))]
[RequireComponent(typeof(PlayerNetworkManager))]
[RequireComponent(typeof(PlayerStatsManager))]

[RequireComponent(typeof(PlayerInventoryManager))]
[RequireComponent(typeof(PlayerEquipmentManager))]
[RequireComponent(typeof(PlayerCombatManager))]
public class PlayerManager : CharacterManager
{
    private PlayerLocomotionManager playerLocomotionManager;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerNetworkManager playerNetworkManager;
    private PlayerStatsManager playerStatsManager;
    private PlayerInventoryManager playerInventoryManager;
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerCombatManager playerCombatManager;

    [Header("DEBUG MENU")]
    [SerializeField] private bool respawnPlayer = false;

    public PlayerLocomotionManager PlayerLocomotionManager => playerLocomotionManager;
    public PlayerAnimatorManager PlayerAnimatorManager => playerAnimatorManager;
    public PlayerNetworkManager PlayerNetworkManager => playerNetworkManager;
    public PlayerStatsManager PlayerStatsManager => playerStatsManager;
    public PlayerInventoryManager PlayerInventoryManager => playerInventoryManager;
    public PlayerEquipmentManager PlayerEquipmentManager => playerEquipmentManager;
    public PlayerCombatManager PlayerCombatManager => playerCombatManager;


    #region CharacterNetworkManager Variables
    public FixedString64Bytes CharacterName
    {
        get => playerNetworkManager.CharacterName;
        set => playerNetworkManager.CharacterName = value;
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
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
            DebugMenu();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    // private void OnClientConnectedCallback(ulong clientID)
    // {
    //     WorldGameSessionManager.GetInstance.AddPlayerToActivePlayerList(this);

    //     // the server is the host, and the host loads the proxies first =>
    //     // the clients are the one who need to "catch up" with the host
    //     if (!IsServer && IsOwner)
    //     {
    //         foreach (var player in WorldGameSessionManager.GetInstance.PlayersList)
    //         {
    //             if (player != this)
    //             {
    //                 player.LoadPlayerCharacterOnJoin();
    //             }
    //         }
    //     }
    // }

    // private void LoadPlayerCharacterOnJoin()
    // {
    //     playerNetworkManager.OnCurrentMainHandWeaponIDChange(0, playerNetworkManager.CurrentMainHandWeaponID.Value); 
    //     playerNetworkManager.OnCurrentOffHandWeaponIDChange(0, playerNetworkManager.CurrentOffHandWeaponID.Value); 

    //     if (playerNetworkManager.IsLockedOn.Value)
    //     {
    //         playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.CurrentTargetNetworkObjectID.Value);
    //     }
    // }

    // public override void OnNetworkSpawn()
    // {
    //     base.OnNetworkSpawn();
    //     NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

    //     if (IsOwner)
    //     {
    //         PlayerCamera.GetInstance.SetPlayer(this);
    //         PlayerInputManager.GetInstance.SetPlayer(this);
    //         WorldSaveGameManager.GetInstance.SetPlayer(this);

    //         // UPDATE THE MAX BAR AMOUNT WHEN THE CORESPONDING STAT CHANGES
    //         Vitality.OnValueChanged += SetNewMaxHealthValue;
    //         Endurance.OnValueChanged += SetNewMaxStaminaValue;

    //         // UPDATES UI STAT BARS WHEN A STAT CHANGES
    //         CurrentHealth.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewHealthValue;
    //         CurrentStamina.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
    //         CurrentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
    //     }

    //     // STATS
    //     CurrentHealth.OnValueChanged += playerNetworkManager.CheckHP;

    //     // EQUIPMENT
    //     playerNetworkManager.CurrentMainHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentMainHandWeaponIDChange;
    //     playerNetworkManager.CurrentOffHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentOffHandWeaponIDChange;
    //     playerNetworkManager.WeaponInUseID.OnValueChanged += playerNetworkManager.OnWeaponInUseIDChange;

    //     // LOCK ON
    //     playerNetworkManager.IsLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChange;
    //     playerNetworkManager.CurrentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

    //     // FLAGS
    //     playerNetworkManager.IsChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChange;

    //     if (IsOwner && !IsServer)
    //     {
    //         LoadCharacterSaveData(WorldSaveGameManager.GetInstance.GetCurrentCharacterSave());
    //     }
    // }

    // public override void OnNetworkDespawn()
    // {
    //     base.OnNetworkDespawn();

    //     NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

    //     if (IsOwner)
    //     {
    //         // UPDATE THE MAX BAR AMOUNT WHEN THE CORESPONDING STAT CHANGES
    //         Vitality.OnValueChanged -= SetNewMaxHealthValue;
    //         Endurance.OnValueChanged -= SetNewMaxStaminaValue;

    //         // UPDATES UI STAT BARS WHEN A STAT CHANGES
    //         CurrentHealth.OnValueChanged -= PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewHealthValue;
    //         CurrentStamina.OnValueChanged -= PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
    //         CurrentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
    //     }

    //     // STATS
    //     CurrentHealth.OnValueChanged -= playerNetworkManager.CheckHP;

    //     // EQUIPMENT
    //     playerNetworkManager.CurrentMainHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentMainHandWeaponIDChange;
    //     playerNetworkManager.CurrentOffHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentOffHandWeaponIDChange;
    //     playerNetworkManager.WeaponInUseID.OnValueChanged -= playerNetworkManager.OnWeaponInUseIDChange;

    //     // LOCK ON
    //     playerNetworkManager.IsLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChange;
    //     playerNetworkManager.CurrentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

    //     // FLAGS
    //     playerNetworkManager.IsChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChange;

    //     if (IsOwner && !IsServer)
    //     {
    //         LoadCharacterSaveData(WorldSaveGameManager.GetInstance.GetCurrentCharacterSave());
    //     }
    // }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.GetInstance.HandleAllCameraActions();
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    { 
        if (IsOwner)
        {
            PlayerUIManager.GetInstance.PlayerUIPopUpManager.SendYouDiedPopUp();
        }

        // TODO: CHECK FOR ALLY PLAYERS DEATH, IF ALL ARE DEAD RESPAWN ALL

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);

    }
    public CharacterSaveData GetCharacterSaveData()
    {
        CharacterSaveData characterSaveData = new CharacterSaveData()
        {
            //TODO: character name
            CharacterName = "GOSU",

            XPosition = transform.position.x,
            YPosition = transform.position.y,
            ZPosition = transform.position.z,

            Vitality = Vitality.Value,
            Endurance = Endurance.Value,

            CurrentHealth = CurrentHealth.Value,
            CurrentStamina = CurrentStamina.Value,
        };

        return characterSaveData;
    }

    public void LoadCharacterSaveData(CharacterSaveData characterSaveData)
    {
        CharacterName = characterSaveData.CharacterName;
        Vector3 playerPosition = new(characterSaveData.XPosition, characterSaveData.YPosition, characterSaveData.ZPosition);
        // transform.position = playerPosition;
        Move(playerPosition);

        Vitality.Value = characterSaveData.Vitality;
        Endurance.Value = characterSaveData.Endurance;

        int health = CharacterStatsManager.CalculateHealthBasedOnVitalityLevel(characterSaveData.Vitality);
        MaxHealth.Value = health;
        PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxHealthValue(health);
        CurrentHealth.Value = characterSaveData.CurrentHealth;

        int stamina = CharacterStatsManager.CalculateStaminaBasedOnEnduranceLevel(characterSaveData.Endurance);
        MaxStamina.Value = stamina;
        PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxStaminaValue(stamina);
        CurrentStamina.Value = characterSaveData.CurrentStamina;
    }

    // public void SetNewMaxHealthValue(int oldValue, int newValue)
    // {
    //     MaxHealth.Value = CharacterStatsManager.CalculateHealthBasedOnVitalityLevel(newValue);
    //     PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxHealthValue(MaxHealth.Value);
    //     CurrentHealth.Value = MaxHealth.Value;
    // }

    // public void SetNewMaxStaminaValue(int oldValue, int newValue)
    // {
    //     MaxStamina.Value = CharacterStatsManager.CalculateStaminaBasedOnEnduranceLevel(newValue);
    //     PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxStaminaValue(MaxStamina.Value);
    //     CurrentStamina.Value = MaxStamina.Value;
    // }

    // DELETE LATER
    private void DebugMenu()
    {
        if (respawnPlayer)
        {
            respawnPlayer = false;
            ReviveCharacter();
        }
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            IsDead.Value = false;
            CurrentHealth.Value = MaxHealth.Value;
            CurrentStamina.Value = MaxStamina.Value;
            // TODO: 
            // RESET FOCUS
            // PLAY REBIRTH ANIMATION
            
            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        }
    }
}
