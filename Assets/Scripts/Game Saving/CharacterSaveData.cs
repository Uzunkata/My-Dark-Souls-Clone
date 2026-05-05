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

    [Header("Stats")]
    [SerializeField] private int vitality;
    [SerializeField] private int endurance;

    [Header("Resources")]
    [SerializeField] private int currentHealth;
    [SerializeField] private float currentStamina;

    #region VARIABLES GETTERS AND SETTERS
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
    public int Vitality
    {
        get => vitality;
        set => vitality = value;
    }
    public int Endurance
    {
        get => endurance;
        set => endurance = value;
    }
    public int CurrentHealth    
    { 
        get => currentHealth; 
        set => currentHealth = value; 
    }
    public float CurrentStamina    
    { 
        get => currentStamina; 
        set => currentStamina = value; 
    }

    #endregion
}
