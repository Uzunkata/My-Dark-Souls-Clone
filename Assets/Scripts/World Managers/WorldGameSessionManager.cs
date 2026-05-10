using System.Collections.Generic;
using UnityEngine;

public class WorldGameSessionManager : MonoBehaviour
{
    private static WorldGameSessionManager instance;

    public static WorldGameSessionManager GetInstance => instance;

    [Header("Active Players In Session")]
    [SerializeField] private List<PlayerManager> players = new();

    public List<PlayerManager> PlayersList => players;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPlayerToActivePlayerList(PlayerManager player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }

        ClearNullsFromList();
    }

    public void RemovePlayerFromActivePlayerList(PlayerManager player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
        
        ClearNullsFromList();
    }

    private void ClearNullsFromList()
    {
        for (int i = players.Count - 1; i > -1; i--)
        {
            if (players[i] == null)
                players.RemoveAt(i);
        }
    }
}
