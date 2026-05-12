using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
[RequireComponent(typeof(PlayerManager))]

[RequireComponent(typeof(PlayerStatsManager))]
public class PlayerNetworkManager : CharacterNetworkManager
{
    private PlayerManager player;
    private PlayerStatsManager playerStatsManager;

    private NetworkVariable<FixedString64Bytes> characterName = new("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipemnt")]
    private NetworkVariable<int> currentMainHandWeaponID = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> currentOffHandWeaponID = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<int> weaponInUseID = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<bool> isUsingMainHand = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<bool> isUsingOffHand = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public FixedString64Bytes CharacterName
    {
        get => characterName.Value;
        set => characterName.Value = value;
    }

    public NetworkVariable<int> CurrentMainHandWeaponID => currentMainHandWeaponID;
    public NetworkVariable<int> CurrentOffHandWeaponID => currentOffHandWeaponID;
    public NetworkVariable<int> WeaponInUseID => weaponInUseID;
    public NetworkVariable<bool> IsUsingMainHand => isUsingMainHand;
    public NetworkVariable<bool> IsUsingOffHand => isUsingOffHand;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if (IsOwner)
        {
            PlayerCamera.GetInstance.SetPlayer(player);
            PlayerInputManager.GetInstance.SetPlayer(player);
            WorldSaveGameManager.GetInstance.SetPlayer(player);

            // UPDATE THE MAX BAR AMOUNT WHEN THE CORESPONDING STAT CHANGES
            Vitality.OnValueChanged += SetNewMaxHealthValue;
            Endurance.OnValueChanged += SetNewMaxStaminaValue;

            // UPDATES UI STAT BARS WHEN A STAT CHANGES
            CurrentHealth.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewHealthValue;
            CurrentStamina.OnValueChanged += PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
            CurrentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
        }

        // STATS
        CurrentHealth.OnValueChanged += CheckHP;

        // EQUIPMENT
        CurrentMainHandWeaponID.OnValueChanged += OnCurrentMainHandWeaponIDChange;
        CurrentOffHandWeaponID.OnValueChanged += OnCurrentOffHandWeaponIDChange;
        WeaponInUseID.OnValueChanged += OnWeaponInUseIDChange;

        // LOCK ON
        IsLockedOn.OnValueChanged += OnIsLockedOnChange;
        CurrentTargetNetworkObjectID.OnValueChanged += OnLockOnTargetIDChange;

        // FLAGS
        IsChargingAttack.OnValueChanged += OnIsChargingAttackChange;

        if (IsOwner && !IsServer)
        {
            player.LoadCharacterSaveData(WorldSaveGameManager.GetInstance.GetCurrentCharacterSave());
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

        if (IsOwner)
        {
            // UPDATE THE MAX BAR AMOUNT WHEN THE CORESPONDING STAT CHANGES
            Vitality.OnValueChanged -= SetNewMaxHealthValue;
            Endurance.OnValueChanged -= SetNewMaxStaminaValue;

            // UPDATES UI STAT BARS WHEN A STAT CHANGES
            CurrentHealth.OnValueChanged -= PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewHealthValue;
            CurrentStamina.OnValueChanged -= PlayerUIManager.GetInstance.PlayerUIHudManager.SetNewStaminaValue;
            CurrentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
        }

        // STATS
        CurrentHealth.OnValueChanged -= CheckHP;

        // EQUIPMENT
        CurrentMainHandWeaponID.OnValueChanged -= OnCurrentMainHandWeaponIDChange;
        CurrentOffHandWeaponID.OnValueChanged -= OnCurrentOffHandWeaponIDChange;
        WeaponInUseID.OnValueChanged -= OnWeaponInUseIDChange;

        // LOCK ON
        IsLockedOn.OnValueChanged -= OnIsLockedOnChange;
        CurrentTargetNetworkObjectID.OnValueChanged -= OnLockOnTargetIDChange;

        // FLAGS
        IsChargingAttack.OnValueChanged -= OnIsChargingAttackChange;

        if (IsOwner && !IsServer)
        {
            player.LoadCharacterSaveData(WorldSaveGameManager.GetInstance.GetCurrentCharacterSave());
        }
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        WorldGameSessionManager.GetInstance.AddPlayerToActivePlayerList(player);

        // the server is the host, and the host loads the proxies first =>
        // the clients are the one who need to "catch up" with the host
        if (!IsServer && IsOwner)
        {
            foreach (var player in WorldGameSessionManager.GetInstance.PlayersList)
            {
                if (player != this)
                {
                    LoadPlayerCharacterOnJoin();
                }
            }
        }
    }

    private void LoadPlayerCharacterOnJoin()
    {
        OnCurrentMainHandWeaponIDChange(0, CurrentMainHandWeaponID.Value); 
        OnCurrentOffHandWeaponIDChange(0, CurrentOffHandWeaponID.Value); 

        if (IsLockedOn.Value)
        {
            OnLockOnTargetIDChange(0, CurrentTargetNetworkObjectID.Value);
        }
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
    public void OnCurrentMainHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerInventoryManager.CurrentMHWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadMainHandWeapon();   

        if (player.IsOwner)
        {
            PlayerUIManager.GetInstance.PlayerUIHudManager.SetMainWeaponQuickSlotIcon(newID);
        }   
    }
    public void OnCurrentOffHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerInventoryManager.CurrentOHWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadOffHandWeapon();

        if (player.IsOwner)
        {
            PlayerUIManager.GetInstance.PlayerUIHudManager.SetOffWeaponQuickSlotIcon(newID);
        }   
    }
    public void OnWeaponInUseIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerCombatManager.WeaponInUse = newWeapon;
        player.PlayerEquipmentManager.LoadOffHandWeapon();  
    }

    public void SetWeaponID(int id, WeaponItem.WeaponModelSlot slot)
    {
        if (id < 0)
            return;

        switch (slot)
        {
            case WeaponItem.WeaponModelSlot.OffHand:
                currentOffHandWeaponID.Value = id;
                break;
            case WeaponItem.WeaponModelSlot.MainHand:
                currentMainHandWeaponID.Value = id;
                break;
            default:
                return;
        }
    }
    
    // this does not necesarily mean that we are using only one of the hands,
    // it just means that we are using the action on the coresponding weapon
    // (for example, powerstance actions)
    public void SetCharacterActionHand(bool isMainHandedAction)
    {
        if (isMainHandedAction)
        {
            isUsingMainHand.Value = true;
            isUsingOffHand.Value = false;
        }
        else
        {
            isUsingMainHand.Value = false;
            isUsingOffHand.Value = true;
        }
    }

    [ServerRpc]
    public void NotifyTheServerOfWeaponActionAnimationServerRpc(ulong clientID, int actionID, int weaponID)
    {
        if (IsServer)
        {
            NotifyTheServerOfWeaponActionAnimationClientRpc(clientID, actionID, weaponID);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfWeaponActionAnimationClientRpc(ulong clientID, int actionID, int weaponID)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponActionAnimationFromServer(actionID, weaponID);
        }
    }

    protected void PerformWeaponActionAnimationFromServer(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionManager.GetInstance.GetWeaponItemActionByID(actionID);
        WeaponItem weapon = WorldItemDatabase.GetInstance.GetWeaponByID(weaponID);

        if (weaponAction != null && weapon != null)
        {
            weaponAction.AttemptToPerformAction(player, weapon);
        }
        else
        {
            Debug.Log("actionID: [" + actionID + "], or weaponID: [" + weaponID + "] does not exist!");
        }
    }
}
