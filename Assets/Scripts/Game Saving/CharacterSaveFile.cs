using UnityEngine;
using System;
using System.IO;

public class CharacterSaveFile
{
    private string directoryPath = "";
    private string fileName = "";

    public string DirectoryPath
    {
        get => directoryPath; 
        set => directoryPath = value;
    }
    public string FileName
    {
        get => fileName; 
        set => fileName = value;
    }

    public bool Exists()
    {
        if (File.Exists(Path.Combine(directoryPath, fileName)))
            return true;

        return false;
    }

    public void Delete()
    {
        File.Delete(Path.Combine(directoryPath, fileName));
    }

    public void CreateCharacterSaveFile(CharacterSaveData characterData)
    {
        string savePath = Path.Combine(directoryPath, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Crating character save file at: " + savePath);

            //serialize
            string dataToStore = JsonUtility.ToJson(characterData);

            //write
            using FileStream stream = new(savePath, FileMode.Create);
            using StreamWriter fileWriter = new(stream);
            fileWriter.Write(dataToStore);
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR whiel trying to save file: " + savePath + "\n" + ex);
        }
    }

    public CharacterSaveData LoadCharacterSaveFile()
    {
        CharacterSaveData characterData = null;
        string loadPath = Path.Combine(directoryPath, fileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad;

                using FileStream stream = new(loadPath, FileMode.Open);
                using StreamReader reader = new(stream);
                dataToLoad = reader.ReadToEnd();

                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch(Exception ex)
            {
                characterData = null;
                Debug.LogError("ERROR whiel trying to load file: " + loadPath + "\n" + ex);
            }
        }
        return characterData;
    }

    public void DeleteCharacterSaveFile()
    {
        string filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
