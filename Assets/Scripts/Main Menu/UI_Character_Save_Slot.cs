using UnityEngine;
using TMPro;

public class UI_Character_Save_Slot : MonoBehaviour
{

    [Header("Save Slot")]
    private int slotIndex;

    [Header("Character Info")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI timePlayed;

    public int SlotIndex
    {
        set => slotIndex = value;
        get => slotIndex;
    } 

    public void LoadSaveSlot(int slotIndex, CharacterSaveData characterSaveData)
    {
        this.slotIndex = slotIndex;
        characterName.text = characterSaveData.CharacterName;
    }

    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.GetInstance.LoadCharacterSave(slotIndex);
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManager.GetInstance.SelectCharacterSlot(slotIndex);
    }
}
