using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterManager))]
public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    public Vector3 networkPositionVelocity;
    [SerializeField]
    private float networkPositionSmoothTime = 0.1f;
    [SerializeField]
    private float networkRotationSmoothTime = 0.1f;

    [Header("Position")]
    private readonly NetworkVariable<Vector3> networkPosition 
        = new (
            Vector3.zero, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner);

    [Header("Rotation")]
    private readonly NetworkVariable<Quaternion> networkRotation 
        = new (
            Quaternion.identity, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner);

    [Header("Animator")]
    private readonly NetworkVariable<float> verticalMovement = 
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<float> horizontalMovement = 
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    private readonly NetworkVariable<float> moveAmount = 
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

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

    private void PerformActionAnimationFromServer(string targetAnimation, bool applyRootMotion)
    {
        character.ApplyRootMotion = applyRootMotion;
        character.Animator.CrossFade(targetAnimation, CharacterAnimatorManager.normalizedTransitionDuration);
    }

    public float NetworkPositionSmoothTime => networkPositionSmoothTime;
    public float NetworkRotationSmoothTime => networkRotationSmoothTime;

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
}
