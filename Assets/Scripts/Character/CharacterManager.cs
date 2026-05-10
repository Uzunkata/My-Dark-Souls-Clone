using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterNetworkManager))]
[RequireComponent(typeof(Animator))]

[RequireComponent(typeof(CharacterEffectsManager))]
[RequireComponent(typeof(CharacterAnimatorManager))]

[RequireComponent(typeof(CharacterLocomotionManager))]
[RequireComponent(typeof(CharacterCombatManager))]
[RequireComponent(typeof(CharacterSoundFXManager))]
public class CharacterManager : NetworkBehaviour
{
    protected CharacterController characterController;
    protected Animator animator;
    protected CharacterNetworkManager characterNetworkManager;
    protected CharacterEffectsManager characterEffectsManager;
    protected CharacterAnimatorManager characterAnimatorManager;
    protected CharacterCombatManager characterCombatManager;
    protected CharacterSoundFXManager characterSoundFXManager;

    [Header("Flags")]
    [SerializeField] protected bool isPerformingAction = false;
    [SerializeField] protected bool applyRootMotion = false;
    [SerializeField] protected bool canRotate = true;
    [SerializeField] protected bool canMove = true;
    [SerializeField] protected bool isGrounded = true;

    public Animator Animator => animator;
    public CharacterNetworkManager CharacterNetworkManager => characterNetworkManager;
    public CharacterEffectsManager CharacterEffectsManager => characterEffectsManager;
    public CharacterAnimatorManager CharacterAnimatorManager => characterAnimatorManager;
    public CharacterCombatManager CharacterCombatManager => characterCombatManager;
    public CharacterSoundFXManager CharacterSoundFXManager => characterSoundFXManager;


    #region ENCAPSULATION

    public bool IsPerformingAction
    {
        get => isPerformingAction;
        set => isPerformingAction = value;
    } 
    public bool ApplyRootMotion
    {
        get => applyRootMotion;
        set => applyRootMotion = value;
    } 
    public bool CanRotate
    {
        get => canRotate;
        set => canRotate = value;
    } 
    public bool CanMove
    {
        get => canMove;
        set => canMove = value;
    } 

    #endregion

    #region Passing CharacterNetworkManager Variables

    // public Vector3 NetworkPosition
    // {
    //     get => networkPosition.Value;
    //     set => networkPosition.Value = value;
    // }
    // public Quaternion NetworkRotation
    // {
    //     get => networkRotation.Value;
    //     set => networkRotation.Value = value;
    // }
    // public float VerticalMovement
    // {
    //     get => verticalMovement.Value;
    //     set => verticalMovement.Value = value;
    // }
    // public float HorizontalMovement
    // {
    //     get => horizontalMovement.Value;
    //     set => horizontalMovement.Value = value;
    // }
    // public float MoveAmount
    // {
    //     get => moveAmount.Value;
    //     set => moveAmount.Value = value;
    // }
    public bool IsSprinting
    {
        set => characterNetworkManager.IsSprinting = value;
        get => characterNetworkManager.IsSprinting;
    }
    public bool IsJumping
    {
        get => characterNetworkManager.IsJumping.Value;
        set => characterNetworkManager.IsJumping.Value = value;
    } 
    public bool IsGrounded
    {
        get => isGrounded;
        set => isGrounded = value;
    } 
    public NetworkVariable<bool> IsDead
    {
        get => characterNetworkManager.IsDead;
    }
    public NetworkVariable<int> Vitality
    {
        get => characterNetworkManager.Vitality;
    }
    public NetworkVariable<int> CurrentHealth => characterNetworkManager.CurrentHealth;
    public NetworkVariable<int> MaxHealth => characterNetworkManager.MaxHealth;
    public NetworkVariable<int> Endurance
    {
        get => characterNetworkManager.Endurance;
    }
    public NetworkVariable<float> CurrentStamina
    {
        get => characterNetworkManager.CurrentStamina;
    }
    public NetworkVariable<int> MaxStamina
    {
        get => characterNetworkManager.MaxStamina;
    }
    
    #endregion

    //Since our player gameObjects are connected to the Network Manager,
    //they get created with it when we host a game (so when we start the game).
    //we want to preserve this player model for other scenes, so that we can maintain
    //the connection.
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
    }

    protected virtual void Update() 
    {
        //if this character is being controlled form our side
        //then assign its position to the network position
        if (IsOwner)
        {
            characterNetworkManager.NetworkPosition = transform.position;
            characterNetworkManager.NetworkRotation = transform.rotation;
            //characterNetworkManager.IsGrounded = isGrounded;
        }
        //if this character is being controlled from else where,
        //then assing its position here localy by the position of its
        //network transform
        else
        {
            //  Position
            transform.position 
                = Vector3.SmoothDamp(
                    transform.position, 
                    characterNetworkManager.NetworkPosition, 
                    ref characterNetworkManager.networkPositionVelocity,        //TODO: what do i do about this????
                    characterNetworkManager.NetworkPositionSmoothTime);
            //  Rotation
            transform.rotation
                = Quaternion.Slerp(
                    transform.rotation, 
                    characterNetworkManager.NetworkRotation, 
                    characterNetworkManager.NetworkRotationSmoothTime);

            //isGrounded = characterNetworkManager.IsGrounded;
        }
    }

    protected virtual void Start()
    {
        if (!IsOwner)
        {
            characterController.enabled = false;
        }

        IgnoreMyOwnColliders();
    }
    public void EnterDefaultFlagState()
    {
        isPerformingAction = false;
        applyRootMotion = false;
        canMove = true;
        canRotate = true;

        if (IsOwner)
        {
            IsJumping = false;
        }
    }

    public void Move(Vector3 moveDirection)
    {
        characterController.Move(moveDirection);
    }

    protected virtual void LateUpdate()
    {
        
    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            CurrentHealth.Value = 0;
            IsDead.Value = true;
        

        // TODO:
        // MANAGE FLAGS
        // IF WE ARE NOT GROUNDED, PLAY A AERIAL DEATH ANIMATION

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Death_01", true);
            }
        }

        // TODO:
        // SOUND FX

        yield return new WaitForSeconds(5);

        // AWARD PVP PLAYERS WITH RINES
        // DISABLE CHARACTER
    }

    public virtual void ReviveCharacter()
    {
        
    }

    protected virtual void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damagageableCharacterColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new();

        foreach (var collider in damagageableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterControllerCollider);

        foreach(var collider in ignoreColliders)
        {
            foreach(var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
}
