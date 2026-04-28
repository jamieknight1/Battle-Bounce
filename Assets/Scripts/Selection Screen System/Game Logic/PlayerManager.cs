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
        //DontDestroyOnLoad(gameObject);
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
            PlayerCard currentCard = GetNextAvailableCard();
            newPlayerData.SetPlayerCard(currentCard);
            HandCursor newHandCursorScript = newHandCursor.GetComponent<HandCursor>();
            newHandCursorScript.InitializePlayerData(newPlayerData);
            newHandCursorScript.SetCursorIconImage(cursorIconSprite[players.Count -1]);
            newHandCursorScript.ChangeHoldingState(HoldingState.HoldingPlayer);
            currentCard.SetCursor(newHandCursor);
            currentCard.InitializeHandCursorScript();
            currentCard.AddPlayer(newPlayerData);
        }
    }

    public void AddCpuPlayer(HandCursor selectingPlayer, PlayerCard cpuCard)
    {
        if (players.Count < 4)
        {
            var newPlayer = new PlayerData(players.Count, PlayerType.CPU);
            var newCursor = Instantiate(cpuCursor, selectingPlayer.transform);
            var newCursorScript = newCursor.GetComponent<CpuCursor>();

            players.Add(newPlayer);
            newPlayer.SetPlayerCard(cpuCard);
            newCursorScript.InitializePlayerData(newPlayer);
            selectingPlayer.SetCpuCursor(newCursorScript);
            selectingPlayer.SetCpuCard(cpuCard);
            cpuCard.AddPlayer(newPlayer);
            newPlayer.SetCpuIcon(newCursorScript);
        }
    }

    public void RemovePlayer(PlayerData playerToRemove)
    {
        if (playerToRemove.PlayerType == PlayerType.CPU) Destroy(playerToRemove.CpuIcon.gameObject);
        else if (playerToRemove.PlayerType == PlayerType.Human) Destroy(playerToRemove.PlayerCard.cursor.gameObject);

        var card = playerToRemove.PlayerCard;
        players.Remove(playerToRemove);
        if (card != null) card.RemovePlayer();
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

    public PlayerCard GetNextAvailableCard()
    {
        foreach (PlayerCard card in playerCards)
        {
            if (card.player == null)
            {
                return card;
            }
        }

        return null;
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
