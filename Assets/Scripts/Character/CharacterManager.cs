using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterNetworkManager))]
[RequireComponent(typeof(Animator))]
public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] protected CharacterController characterController;
    [HideInInspector] protected Animator animator;
    [HideInInspector] private CharacterNetworkManager characterNetworkManager;

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
    }

    protected virtual void Update() 
    {
        //if this character is being controlled form our side
        //then assign its position to the network position
        if (IsOwner)
        {
            characterNetworkManager.SetNetworkPosition(transform.position);
            characterNetworkManager.SetNetworkRotation(transform.rotation);
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
                    characterNetworkManager.GetNetworkPosition(), 
                    ref characterNetworkManager.networkPositionVelocity,        //TODO: what do i do about this????
                    characterNetworkManager.NetworkPositionSmoothTime);
            //  Rotation
            transform.rotation
                = Quaternion.Slerp(
                    transform.rotation, 
                    characterNetworkManager.GetNetworkRotation(), 
                    characterNetworkManager.NetworkRotationSmoothTime);
        }
    }

    public void Move(Vector3 moveDirection)
    {
        characterController.Move(moveDirection);
    }

    protected virtual void LateUpdate()
    {
        
    }
    public Animator Animator => animator;
    public CharacterNetworkManager CharacterNetworkManager => characterNetworkManager;

}
