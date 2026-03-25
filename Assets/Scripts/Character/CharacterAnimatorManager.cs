using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterAnimatorManager : MonoBehaviour
{
    [HideInInspector] private CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontal, float vertical)
    {
        character.Animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        character.Animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
    }

}
