using UnityEngine;

public class LoadGameMenuManager : MonoBehaviour
{
    private PlayerControls playerControls;

    [Header("Title Screen Inputs")]
    [SerializeField] private bool deleteCharacterSlot = false;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.DeleteSave.performed += i => deleteCharacterSlot = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        if (deleteCharacterSlot)
        {
            deleteCharacterSlot = false;
            TitleScreenManager.GetInstance.AttemptToDeleteSelectedCharacterSlot();
        }
    }
}
