using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterManager))]
public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    public Vector3 networkPositionVelocity;
    [SerializeField]
    protected float networkPositionSmoothTime = 0.1f;
    [SerializeField]
    protected float networkRotationSmoothTime = 0.1f;

    [Header("Position")]
    protected NetworkVariable<Vector3> networkPosition = new (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Rotation")]
    protected NetworkVariable<Quaternion> networkRotation = new (Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Animator")]
    protected NetworkVariable<float> verticalMovement = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<float> horizontalMovement = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected NetworkVariable<float> moveAmount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    protected NetworkVariable<bool> isSprinting = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    [SerializeField] protected NetworkVariable<int> endurance = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> maxStamina = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public Vector3 NetworkPosition
    {
        get => networkPosition.Value;
        set => networkPosition.Value = value;
    }
    public Quaternion NetworkRotation
    {
        get => networkRotation.Value;
        set => networkRotation.Value = value;
    }
    public float VerticalMovement
    {
        get => verticalMovement.Value;
        set => verticalMovement.Value = value;
    }
    public float HorizontalMovement
    {
        get => horizontalMovement.Value;
        set => horizontalMovement.Value = value;
    }
    public float MoveAmount
    {
        get => moveAmount.Value;
        set => moveAmount.Value = value;
    }
    public bool IsSprinting
    {
        set => isSprinting.Value = value;
        get => isSprinting.Value;
    }
    public int Endurance
    {
        get => endurance.Value;
        set => endurance.Value = value;
    }
    
    public NetworkVariable<float> GetCurrentStamina()
    {
        return currentStamina;
    }

    public NetworkVariable<float> GetMaxStamina()
    {
        return maxStamina;
    }

    protected virtual void Awake() 
    {
        character = GetComponent<CharacterManager>();
    }

    //a server RPC is a funciton called from a client, but executed on the server(the host).
    //Also, the reason we require the root motion form a client (the host is also a client),
    //is because the animations will be smoother if we calculate them localy for everyone.
    //Note that we don't requre other parameters because (positions, rotations, etc) are
    //calculated localy and then are shared via NetworkVariable's
    [ServerRpc]
    public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string targetAnimation, bool applyRootMotion)
    {
        //if we are the server, play action animation for all clients
        if (IsServer)
        {
            PlayActionAnimationForAllClientsClientRpc(clientID, targetAnimation, applyRootMotion);
        }
    }

    //a client RPC is a function called by the server, but executed by all clients
    [ClientRpc]
    public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string targetAnimation, bool applyRootMotion)
    {
        //we make sure to not run the function on the player who sent it
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServer(targetAnimation, applyRootMotion);
        }
    }

    protected void PerformActionAnimationFromServer(string targetAnimation, bool applyRootMotion)
    {
        character.ApplyRootMotion = applyRootMotion;
        character.Animator.CrossFade(targetAnimation, CharacterAnimatorManager.normalizedTransitionDuration);
    }

    public float NetworkPositionSmoothTime => networkPositionSmoothTime;
    public float NetworkRotationSmoothTime => networkRotationSmoothTime;
}

