using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

[RequireComponent(typeof(PlayerManager))]
public class PlayerNetworkManager : CharacterNetworkManager
{
    private PlayerManager player;
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
    }

    public void OnCurrentMainHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerInventoryManager.CurrentMHWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadRightWeapon();      
    }
    public void OnCurrentOffHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerInventoryManager.CurrentOHWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadLeftWeapon();
    }
    public void OnWeaponInUseIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerCombatManager.WeaponInUse = newWeapon;
        player.PlayerEquipmentManager.LoadLeftWeapon();  
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
