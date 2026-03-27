using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerLocomotionManager))]
[RequireComponent(typeof(PlayerAnimatorManager))]
public class PlayerManager : CharacterManager
{
    [HideInInspector] private PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] private PlayerAnimatorManager playerAnimatorManager;

    public PlayerLocomotionManager PlayerLocomotionManager => playerLocomotionManager;
    public PlayerAnimatorManager PlayerAnimatorManager => playerAnimatorManager;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        //if we are not the owner of this objedct (network owner),
        //then let it's owner deal with it's update
        if (!IsOwner)
            return;

        //here we put all the things we want to handle only when we are in the game world
        if (SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.GetInstance.GetWorldSceneIndex())
        {
            playerLocomotionManager.HandleAllMovement();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.GetInstance.SetPlayer(this);
            PlayerInputManager.GetInstance.SetPlayer(this);
        }
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.GetInstance.HandleAllCameraActions();
    }
}
