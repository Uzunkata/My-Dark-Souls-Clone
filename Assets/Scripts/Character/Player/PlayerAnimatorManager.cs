using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerAnimatorManager : CharacterAnimatorManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        if (player.ApplyRootMotion)
        {
            Vector3 velocity = player.Animator.deltaPosition;
            player.Move(velocity);
            player.transform.rotation *= player.Animator.deltaRotation;
        }
    }
}
