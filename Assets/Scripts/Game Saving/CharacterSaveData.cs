using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    [Header("Character Name")]
    [SerializeField] private string characterName;

    [Header("Time played")]
    [SerializeField] private float secondsPlayed;

    //we serializable only basic variable types
    [Header("World Coordinates")]
    [SerializeField] private float xPosition;
    [SerializeField] private float yPosition;
    [SerializeField] private float zPosition;

    [Header("Statuses")]
    [SerializeField] private float currentStamina;

    public string CharacterName 
    { 
        get => characterName; 
        set => characterName = value; 
    }
    public float SecondsPlayed    
    { 
        get => secondsPlayed; 
        set => secondsPlayed = value; 
    }
    public float XPosition     
    { 
        get => xPosition; 
        set => xPosition = value; 
    }
    public float YPosition    
    { 
        get => yPosition; 
        set => yPosition = value; 
    }
    public float ZPosition    
    { 
        get => zPosition; 
        set => zPosition = value; 
    }

    public float CurrentStamina    
    { 
        get => currentStamina; 
        set => currentStamina = value; 
    }
}
