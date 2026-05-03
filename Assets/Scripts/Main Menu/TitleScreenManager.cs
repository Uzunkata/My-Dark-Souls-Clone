using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{

    private static TitleScreenManager instance;

    public static TitleScreenManager GetInstance
    {
        get { return instance; }
    }

    [Header("Menus")]
    [SerializeField] private GameObject titleScreenMainMenu;
    [SerializeField] private GameObject titleScreenLoadMenu;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuNewGameBTN;
    [SerializeField] private Button loadMenuToMainMenuBTN;
    [SerializeField] private Button mainMenuToLoadMenuBTN;
    [SerializeField] private Button deleteCharacterConfirmBTN;
    [SerializeField] private Button deleteCharacterCancleBTN;


    [Header("Pop Ups")]
    [SerializeField] private GameObject noCharacterSaveSlotsPopUp;
    [SerializeField] private Button noCharacterSaveSlotsPopUpBtton;
    [SerializeField] private GameObject deleteCharacterSlotPopUp;

    [Header("Charafter Save Slots")]
    [SerializeField] private int selectedCharacterSlotIndex = -1;

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    public void StartNewGame()
    {
        WorldSaveGameManager.GetInstance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);

        mainMenuToLoadMenuBTN.Select();
        // TODO: find the firs load slot and auto select it
    }

    public void CloseLoadGameMenu()
    {
        titleScreenLoadMenu.SetActive(false);
        titleScreenMainMenu.SetActive(true);

        loadMenuToMainMenuBTN.Select();
    }

    public void DisplayNotFreeCharacterSloptsPopUp()
    {
        noCharacterSaveSlotsPopUp.SetActive(true);
        noCharacterSaveSlotsPopUpBtton.Select();
    }

    public void CloseNotFreeCharacterSloptsPopUp()
    {
        noCharacterSaveSlotsPopUp.SetActive(false);
        mainMenuNewGameBTN.Select();
    }

    // CHARACTER SLOTS
    public void SelectCharacterSlot(int characterSlotIndex)
    {
        selectedCharacterSlotIndex = characterSlotIndex;
    }

    public void UnselectCharacterSaveSlot()
    {
        selectedCharacterSlotIndex = -1;
    }

    public void AttemptToDeleteSelectedCharacterSlot()
    {
        if (selectedCharacterSlotIndex == -1)
            return;
        
        deleteCharacterSlotPopUp.SetActive(true);
        deleteCharacterCancleBTN.Select();

    }

    public void DeleteSelectedCharacterSlot()
    {
        WorldSaveGameManager.GetInstance.DeleteGame(selectedCharacterSlotIndex);

        CloseDeleteCharacterPopUp();

        // we need to refresh the titleScreenLoadMenu in order to update the visability of the caracterslots
        WorldSaveGameManager.GetInstance.ReloadAllCharacterProfiles();
    }

    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuToMainMenuBTN.Select();
    }
}
