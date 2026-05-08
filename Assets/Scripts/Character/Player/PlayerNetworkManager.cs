using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

[RequireComponent(typeof(PlayerManager))]
public class PlayerNetworkManager : CharacterNetworkManager
{
    private PlayerManager player;
    private NetworkVariable<FixedString64Bytes> characterName = new("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipemnt")]
    private NetworkVariable<int> currentRightHandWeaponID = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> currentLeftHandWeaponID = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public FixedString64Bytes CharacterName
    {
        get => characterName.Value;
        set => characterName.Value = value;
    }

    public NetworkVariable<int> CurrentRightHandWeaponID => currentRightHandWeaponID;
    public NetworkVariable<int> CurrentLeftHandWeaponID => currentLeftHandWeaponID;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerInventoryManager.CurrentRHWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadRightWeapon();      
    }

    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.GetInstance.GetWeaponByID(newID));
        player.PlayerInventoryManager.CurrentLHWeapon = newWeapon;
        player.PlayerEquipmentManager.LoadLeftWeapon();
    }

    public void SetWeaponID(int id, WeaponItem.WeaponModelSlot slot)
    {
        if (id < 0)
            return;

        switch (slot)
        {
            case WeaponItem.WeaponModelSlot.LeftHand:
                currentLeftHandWeaponID.Value = id;
                break;
            case WeaponItem.WeaponModelSlot.RightHand:
                currentRightHandWeaponID.Value = id;
                break;
            default:
                return;
        }
    }

    // protected override void PerformActionAnimationFromServer(string targetAnimation, bool applyRootMotion)
    // {
    //     base.PerformActionAnimationFromServer(targetAnimation, applyRootMotion);

    //                                                                                     Debug.Log("I EXIST");
    //     if (!IsOwner && targetAnimation == "Main_Jump_Start_01")
    //     {
    //                                                                                     Debug.Log("I AM IN!");
    //         player.PlayerLocomotionManager.ApplyJumpingVelocity();
    //     }
    // }
}
