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
    [SerializeField] protected NetworkVariable<bool> isDead = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Position")]
    protected NetworkVariable<Vector3> networkPosition = new (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Rotation")]
    protected NetworkVariable<Quaternion> networkRotation = new (Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<ulong> currentTargetNetworkObjectID = new (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Animator")]
    protected NetworkVariable<float> verticalMovement = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<float> horizontalMovement = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<float> moveAmount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    protected NetworkVariable<bool> isSprinting = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<bool> isJumping = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<bool> isLockedOn = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    [Header("Stats")]
    [SerializeField] protected NetworkVariable<int> vitality = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<int> endurance = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    [SerializeField] protected NetworkVariable<int> currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<int> maxHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<float> currentStamina = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<int> maxStamina = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    #region ENCAPSULATION
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
    public NetworkVariable<bool> IsJumping => isJumping;
    public NetworkVariable<bool> IsLockedOn => isLockedOn;

    public NetworkVariable<ulong> CurrentTargetNetworkObjectID => currentTargetNetworkObjectID;

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

    public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
    {
        if (!IsOwner)
        {
            character.CharacterCombatManager.CurrentLockedOnTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
        }
    }

    public void OnIsLockedOnChange(bool oldValue, bool newValue)
    {
        if (!newValue)
        {
            character.CharacterCombatManager.CurrentLockedOnTarget = null;
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

    // ATTACK ANIMATION
    [ServerRpc]
    public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string targetAnimation, bool applyRootMotion)
    {
        //if we are the server, play action animation for all clients
        if (IsServer)
        {
            PlayAttackActionAnimationForAllClientsClientRpc(clientID, targetAnimation, applyRootMotion);
        }
    }

    [ClientRpc]
    public void PlayAttackActionAnimationForAllClientsClientRpc(ulong clientID, string targetAnimation, bool applyRootMotion)
    {
        //we make sure to not run the function on the player who sent it
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformAttackActionAnimationFromServer(targetAnimation, applyRootMotion);
        }
    }

    protected void PerformAttackActionAnimationFromServer(string targetAnimation, bool applyRootMotion)
    {
        character.ApplyRootMotion = applyRootMotion;
        character.Animator.CrossFade(targetAnimation, CharacterAnimatorManager.normalizedTransitionDuration);
    }

    //DAMAGE
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyTheServerOfCharacterDamageServerRpc(
        ulong characterTakingDMG_ID, 
        ulong characterCausingDMG_ID,
        Damage damage, 
        float angleHitFrom, 
        float contactPointX, 
        float contactPointY, 
        float contactPointZ) 
        {
        if (IsServer)
        {
            NotifyTheServerOfCharacterDamageClientRpc(characterTakingDMG_ID, characterCausingDMG_ID, damage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfCharacterDamageClientRpc(
        ulong characterTakingDMG_ID, 
        ulong characterCausingDMG_ID,
        Damage damage, 
        float angleHitFrom, 
        float contactPointX, 
        float contactPointY, 
        float contactPointZ) 
    {
        ProcessCharacterDamageFromServer(characterTakingDMG_ID, characterCausingDMG_ID, damage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
    }

    public void ProcessCharacterDamageFromServer(
        ulong characterTakingDMG_ID, 
        ulong characterCausingDMG_ID,
        Damage damage, 
        float angleHitFrom, 
        float contactPointX, 
        float contactPointY, 
        float contactPointZ) 
    {
        CharacterManager characterTakingDMG = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterTakingDMG_ID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDMG = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDMG_ID].gameObject.GetComponent<CharacterManager>();
        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.GetInstance.TakeDamageEffect);

        damageEffect.Damage = damage;
        damageEffect.AngleHitFrom = angleHitFrom;
        damageEffect.ContactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
        damageEffect.CharacterCausingDamage = characterCausingDMG;

        characterTakingDMG.CharacterEffectsManager.ProcessEffect(damageEffect);
    }
}

