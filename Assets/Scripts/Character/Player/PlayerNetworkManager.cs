using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

[RequireComponent(typeof(PlayerManager))]
public class PlayerNetworkManager : CharacterNetworkManager
{
    private PlayerManager player;
    private NetworkVariable<FixedString64Bytes> characterName = new("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public FixedString64Bytes CharacterName
    {
        get => characterName.Value;
        set => characterName.Value = value;
    }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }
}
