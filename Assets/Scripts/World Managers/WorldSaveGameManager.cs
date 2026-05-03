using System;
using System.Collections;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    private static WorldSaveGameManager instance;
    [SerializeField] private PlayerManager player;
    [SerializeField] private GameObject save_slot_prefab;
    [SerializeField] private Transform saveSlotsParent; // Content holder
    private string saveDirectory;

    [Header("Save/Load")]
    [SerializeField] private bool saveGame;
    [SerializeField] private bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] private int worldSceneIndex = 1;
    
    [Header("Character Saves")]
    [SerializeField] public static int MAX_CHARACTER_SAVES = 10;
    private CharacterSaveData[] characterSaves = new CharacterSaveData[MAX_CHARACTER_SAVES];
    [SerializeField] private int characterSavesCount = 0;
    [SerializeField] private int currentSlotIndex = 0;

    public static WorldSaveGameManager GetInstance
    {
        get { return instance; }
    }

    public int CharacterSavesCount => characterSavesCount;

    public void SetPlayer(PlayerManager player) { this.player = player; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            OnAwake();
        } 
        else
        {
            //we want only one instance of this script at one time, if another exists, destroy it
            Destroy(gameObject);
        }
    }

    private void OnAwake()
    {
        saveDirectory = Application.persistentDataPath;

        for (int i = 0; i < MAX_CHARACTER_SAVES; i++)
        {
            // to indicate an emtpy save slot
            characterSaves[i] = null;
        }
    }

    //we want this object to stay with us throuh every scene
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    public CharacterSaveData[] GetCharacterSaves()
    {
        return characterSaves;
    }

    public CharacterSaveData GetCurrentCharacterSave()
    {
        return characterSaves[currentSlotIndex];
    }

    private void LoadAllCharacterProfiles()
    {
        characterSavesCount = 0;
        CharacterSaveFile saveFile = new CharacterSaveFile(); 
        saveFile.DirectoryPath = saveDirectory;

        for (int i = 0; i < MAX_CHARACTER_SAVES; i++)
        {
            saveFile.FileName = GetFileName(i);
            characterSaves[i] = saveFile.LoadCharacterSaveFile();

            if (characterSaves[i] != null)
            {
                characterSavesCount++;
                GameObject newSaveSlot = Instantiate(save_slot_prefab, saveSlotsParent);
                UI_Character_Save_Slot save_Slot = newSaveSlot.GetComponent<UI_Character_Save_Slot>();

                save_Slot.LoadSaveSlot(i, characterSaves[i]);
            }
        }
    }

    public void ReloadAllCharacterProfiles()
    {
        foreach (Transform child in saveSlotsParent)
            Destroy(child.gameObject);

        LoadAllCharacterProfiles();
    }

    public void LoadCharacterSave(int slotIndex)
    {
        validateSaveSlotIndex(slotIndex);

        currentSlotIndex = slotIndex;
        LoadGame();
    }

    public void AttemptToCreateNewGame()
    {
        if (characterSavesCount == MAX_CHARACTER_SAVES)
        {
            TitleScreenManager.GetInstance.DisplayNotFreeCharacterSloptsPopUp();
            return;
        }

        int emptySlot = -1;
        for (int i = 0; i < MAX_CHARACTER_SAVES; i++)
        {
            if (characterSaves[i] == null)
            {
                emptySlot = i;
                break;
            }
        }

        currentSlotIndex = emptySlot;
        characterSaves[currentSlotIndex] = new CharacterSaveData();
        characterSavesCount++;
        SaveGame();
        StartCoroutine(LoadWorldScene());
    }

    public string GetFileName(int index)
    {
        try
        {
            validateSaveSlotIndex(index);
        } catch(Exception)
        {
            return null;
        }

        return "Character_Save_" + index;

    }

    private void validateSaveSlotIndex(int saveSlotIndex)
    {
        if (saveSlotIndex < 0 || saveSlotIndex >= MAX_CHARACTER_SAVES)
        {
            throw new Exception("WorldSaveGameManager::DeleteGame(...) -> invalid characterSlotIndex given");
        }
    }
    private CharacterSaveFile getSaveFileBasedOnIndex(int saveSlotIndex)
    {
        validateSaveSlotIndex(saveSlotIndex);

        return new()
        {
            DirectoryPath = saveDirectory,
            FileName = GetFileName(saveSlotIndex)
        };
    }
    public void LoadGame()
    {
        CharacterSaveFile saveFileDataWriter = getSaveFileBasedOnIndex(currentSlotIndex);
        characterSaves[currentSlotIndex] = saveFileDataWriter.LoadCharacterSaveFile();
        player.LoadCharacterSaveData(characterSaves[currentSlotIndex]);

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame()
    {
        CharacterSaveFile saveFileDataWriter = getSaveFileBasedOnIndex(currentSlotIndex);
        characterSaves[currentSlotIndex] = player.GetCharacterSaveData();
        saveFileDataWriter.CreateCharacterSaveFile(characterSaves[currentSlotIndex]);
    }

    public void DeleteGame(int characterSlotIndex)
    {
        validateSaveSlotIndex(characterSlotIndex);

        characterSaves[characterSlotIndex] = null;

        CharacterSaveFile saveFileDataWriter = getSaveFileBasedOnIndex(characterSlotIndex);
        saveFileDataWriter.DeleteCharacterSaveFile();
    }
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

}
