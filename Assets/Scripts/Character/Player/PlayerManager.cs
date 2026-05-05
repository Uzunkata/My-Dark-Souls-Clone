using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Collections;
using System;

[RequireComponent(typeof(PlayerLocomotionManager))]
[RequireComponent(typeof(PlayerAnimatorManager))]
[RequireComponent(typeof(PlayerNetworkManager))]
[RequireComponent(typeof(PlayerStatsManager))]
public class PlayerManager : CharacterManager
{
    private PlayerLocomotionManager playerLocomotionManager;
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerNetworkManager playerNetworkManager;
    private PlayerStatsManager playerStatsManager;

    [SerializeField] private bool respawnPlayer = false;

    public PlayerLocomotionManager PlayerLocomotionManager => playerLocomotionManager;
    public PlayerAnimatorManager PlayerAnimatorManager => playerAnimatorManager;
    public PlayerNetworkManager PlayerNetworkManager => playerNetworkManager;
    public PlayerStatsManager PlayerStatsManager => playerStatsManager;

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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.GetInstance.SetPlayer(this);
            PlayerInputManager.GetInstance.SetPlayer(this);
            WorldSaveGameManager.GetInstance.SetPlayer(this);

            // UPDATE THE MAX BAR AMOUNT WHEN THE CORESPONDING STAT CHANGES
            Vitality.OnValueChanged += SetNewMaxHealthValue;
            Endurance.OnValueChanged += SetNewMaxStaminaValue;

            // UPDATES UI STAT BARS WHEN A STAT CHANGES
            CurrentHealth.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewHealthValue;
            CurrentStamina.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
            CurrentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
        }

        CurrentHealth.OnValueChanged += playerNetworkManager.CheckHP;
    }

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
        transform.position = playerPosition;

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

    public void SetNewMaxHealthValue(int oldValue, int newValue)
    {
        MaxHealth.Value = CharacterStatsManager.CalculateHealthBasedOnVitalityLevel(newValue);
        PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxHealthValue(MaxHealth.Value);
        CurrentHealth.Value = MaxHealth.Value;
    }

    public void SetNewMaxStaminaValue(int oldValue, int newValue)
    {
        MaxStamina.Value = CharacterStatsManager.CalculateStaminaBasedOnEnduranceLevel(newValue);
        PlayerUIManager.GetInstance.PlayerUIHudManager.SetMaxStaminaValue(MaxStamina.Value);
        CurrentStamina.Value = MaxStamina.Value;
    }

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
            CurrentHealth.Value = MaxHealth.Value;
            CurrentStamina.Value = MaxStamina.Value;
            // TODO: 
            // RESET FOCUS
            // PLAY REBIRTH ANIMATION
            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        }
    }
}
