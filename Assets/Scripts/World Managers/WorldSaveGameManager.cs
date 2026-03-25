using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    //singleton pattern
    #region Singleton Patern
    private static WorldSaveGameManager instance;

    public static WorldSaveGameManager GetInstance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            //we want only one instance of this script at one time, if another exists, destroy it
            Destroy(gameObject);
        }
    }

    //we want this object to stay with us throuh every scene
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    [SerializeField]
    private int worldSceneIndex = 1;

    public IEnumerator LoadNewGame()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

}
