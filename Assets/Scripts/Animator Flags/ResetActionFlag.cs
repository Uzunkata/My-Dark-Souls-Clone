using System;
using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    CharacterManager character;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (character == null)
        {
            //here, animator references the animator on the player prefab
            character = animator.GetComponent<CharacterManager>();
        }

        character.EnterDefaultFlagState();
    }
}
