using UnityEngine;

public class DummyCharacter : CharacterManager
{
    protected override void Start()
    {
        base.Start();
        characterController.enabled = true;
        characterLocomotionManager.enabled = false;
    }

}
