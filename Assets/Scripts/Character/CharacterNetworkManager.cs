using UnityEngine;
using Unity.Netcode;
using System;

[RequireComponent(typeof(CharacterManager))]
public class CharacterNetworkManager : NetworkBehaviour
{
    private CharacterManager character;

    public Vector3 networkPositionVelocity;
    [SerializeField] protected float networkPositionSmoothTime = 0.1f;
    [SerializeField] protected float networkRotationSmoothTime = 0.1f;

    [Header("Status")]
    protected NetworkVariable<bool> isDead = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Position")]
    protected NetworkVariable<Vector3> networkPosition = new (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Rotation")]
    protected NetworkVariable<Quaternion> networkRotation = new (Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Animator")]
    protected NetworkVariable<float> verticalMovement = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<float> horizontalMovement = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<float> moveAmount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // protected NetworkVariable<float> inAirTimer = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // [SerializeField] protected NetworkVariable<float> verticalVelocity = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    protected NetworkVariable<bool> isSprinting = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<bool> isJumping = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //[SerializeField] protected NetworkVariable<bool> isGrounded = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    [SerializeField] protected NetworkVariable<int> vitality = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<int> endurance = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    [SerializeField] protected NetworkVariable<int> currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<int> maxHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<float> currentStamina = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<int> maxStamina = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    #region VARIABLES GETTERS AND SETTERS
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
    // public float VerticalVelocity
    // {
    //     get => verticalVelocity.Value;
    //     set => verticalVelocity.Value = value;
    // }

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
    public NetworkVariable<bool> IsJumping => isJumping;
    // public bool IsGrounded
    // {
    //     set => isGrounded.Value = value;
    //     get => isGrounded.Value;
    // }
    // public float InAirTimer
    // {
    //     set => inAirTimer.Value = value;
    //     get => inAirTimer.Value;
    // }

    public float NetworkPositionSmoothTime => networkPositionSmoothTime;
    public float NetworkRotationSmoothTime => networkRotationSmoothTime;
    public NetworkVariable<bool> IsDead => isDead;
    public NetworkVariable<int> Vitality => vitality;
    public NetworkVariable<int> CurrentHealth => currentHealth;

    public NetworkVariable<int> MaxHealth => maxHealth;
    public NetworkVariable<int> Endurance => endurance;
    public NetworkVariable<float> CurrentStamina => currentStamina;
    public NetworkVariable<int> MaxStamina => maxStamina;

    #endregion

    protected virtual void Awake() 
    {
        character = GetComponent<CharacterManager>();
    }

    public void CheckHP(int oldValue, int newValue)
    {
        if (currentHealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }

        // To prevent overhealing
        if (character.IsOwner)
        {
            if (currentHealth.Value > maxHealth.Value)
                currentHealth.Value = maxHealth.Value;
        }
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
}

