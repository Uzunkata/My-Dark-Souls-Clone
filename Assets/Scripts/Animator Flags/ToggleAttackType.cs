using UnityEngine;

public class ToggleAttackType : StateMachineBehaviour
{
    private CharacterManager character;
    [SerializeField] private WeaponItemAction.AttackType attackType;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }

        character.CharacterCombatManager.CurrentAttackType = attackType;
    }

}
