using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNetworkManager : CharacterNetworkManager
{
    private NetworkVariable<FixedString64Bytes> characterName 
        = new("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public FixedString64Bytes CharacterName
    {
        get => characterName.Value;
        set => characterName.Value = value;
    }
}
