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
    private List<PlayerData> players = new List<PlayerData>();
    [SerializeField] GameObject cursorPf;
    [SerializeField] Sprite[] cursorIconSprite;
    [SerializeField] PlayerCard[] playerCards;
    [SerializeField] GameObject cpuCursor;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
            newHandCursorScript.ChangeHoldingState(HoldingState.HoldingPlayer);
            PlayerCard currentCard = playerCards[players.Count - 1];
            currentCard.cursor = newHandCursor;
            currentCard.InitializeHandCursorScript();
        }
    }

    public void AddCpuPlayer(HandCursor selectingPlayer)
    {
        if (players.Count < 4)
        {
            Debug.Log("CPU Player Added");
            players.Add(new PlayerData(players.Count, PlayerType.CPU));
            var newCursor = Instantiate(cpuCursor, selectingPlayer.transform);
            var newCursorScript = newCursor.GetComponent<CpuCursor>();
            newCursorScript.InitializePlayerData(players[players.Count-1]);
            selectingPlayer.SetCpuCursor(newCursorScript);
            selectingPlayer.SetCpuCard(playerCards[players.Count -1]);

            Debug.Log(players.Count);
        }
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

    public IReadOnlyList<PlayerCard> GetPlayerCards()
    {
        return playerCards;
    }
}
