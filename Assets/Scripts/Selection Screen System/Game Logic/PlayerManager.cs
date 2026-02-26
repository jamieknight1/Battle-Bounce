using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance {get; private set;}
    private static List<PlayerData> players = new List<PlayerData>();
    [SerializeField] GameObject cursorPf;
    [SerializeField] Sprite[] cursorIconSprite;
    [SerializeField] PlayerCard[] playerCards;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log($"PlayerManager Instance: {this.GetInstanceID()} PlayerManager Gameobject: {gameObject.name}");
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    public void OnSceneChanged(Scene arg0, Scene arg1)
    {
        Debug.Log("Scene Changed");
        foreach (var player in players)
        {
            Debug.Log(player.Character.name);
        }
    }

    public void UpdatePlayerId()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetPlayerId(i);
        }
    }

    public void AddHumanPlayer(InputDevice device, GameObject newHandCursor)
    {
        if (players.Count < 4) 
        {
            PlayerData newPlayerData = new PlayerData(players.Count, PlayerType.Human, device);
            players.Add(newPlayerData);
            HandCursor newHandCursorScript = newHandCursor.GetComponent<HandCursor>();
            newHandCursorScript.InitializePlayerData(newPlayerData);
            newHandCursorScript.SetCursorIconImage(cursorIconSprite[players.Count -1]);
            PlayerCard currentCard = playerCards[players.Count - 1];
            currentCard.cursor = newHandCursor;
            currentCard.InitializeHandCursorScript();
        }
    }

    public void AddCpuPlayer()
    {
        if (players.Count < 4) players.Add(new PlayerData(players.Count, PlayerType.CPU));
    }

    public void RemovePlayer(PlayerData playerToRemove)
    {
        players.Remove(playerToRemove);
    }

    public bool CanStartMatch()
    {
        if (players.Count < 2) return false;

        foreach (var player in players)
        {
            if (!player.IsLocked)
            {
                return false;
            }
        }
        return true;
    }

    public IReadOnlyList<PlayerData> GetPlayers()
    {
        return players;
    }
}
