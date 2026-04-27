using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectSession : MonoBehaviour
{
    public List<HandCursor> activeCursors = new List<HandCursor>();
    private PlayerManager playerManager;
    private LayerMask playerLayerMask;
    [SerializeField] private GameSetup gameSetup;
    private PlayerCard currentCpuCard;

    void Awake()
    {
        playerLayerMask = LayerMask.GetMask("PlayerObjects");
        playerManager = PlayerManager.Instance;
    }

    public void RegisterCursor(HandCursor cursor)
    {
        if (!activeCursors.Contains(cursor))
        {
            activeCursors.Add(cursor);

            cursor.SelectPressed += OnCursorSelect;
            cursor.StartPressed += OnCursorStart;
            cursor.CancelPressed += OnCursorCancel;
        }
    }

    public void UnregisterCursor(HandCursor cursor)
    {
        cursor.SelectPressed -= OnCursorSelect;
        cursor.StartPressed -= OnCursorStart;
        cursor.CancelPressed -= OnCursorCancel;

        activeCursors.Remove(cursor);
    }

    void OnCursorSelect(HandCursor cursor)
    {
        Collider2D target = Physics2D.OverlapPoint(cursor.transform.position, playerLayerMask, -Mathf.Infinity, Mathf.Infinity);
        if (target != null)
        {
            if (!cursor.PlayerData.IsLocked && target.CompareTag("Player"))
            {
                cursor.PlayerData.AssignCharacter(target.gameObject.GetComponent<CharacterSelectButton>().CharacterPrefab);
                cursor.PlayerData.Lock();
                cursor.LockCursorIconPosition();
                cursor.ChangeHoldingState(HoldingState.HoldingNothing);
            }

            else if (cursor.PlayerData.IsLocked && target.CompareTag("PlayerCard") && cursor.holdingState == HoldingState.HoldingNothing && !target.GetComponent<PlayerCard>().isCpu)
            {
                if (target.GetComponent<PlayerCard>().cursor == null)
                {
                    var cpuCard = target.GetComponent<PlayerCard>();
                    currentCpuCard = cpuCard;
                    cpuCard.isCpu = true;
                    cpuCard.SwitchSprite();
                    cpuCard.SetCursor(cursor.gameObject);
                    cpuCard.InitializeHandCursorScript();
                    playerManager.AddCpuPlayer(cursor, cpuCard);
                    cursor.CloseHand();
                    cursor.ChangeHoldingState(HoldingState.HoldingCPU);
                }
            }

            else if (cursor.holdingState == HoldingState.HoldingCPU && target.CompareTag("Player"))
            {
                currentCpuCard.RemoveCursor();
                currentCpuCard.RemoveCursorScript();
                cursor.cpuCursorIcon.playerData.AssignCharacter(target.gameObject.GetComponent<CharacterSelectButton>().CharacterPrefab);
                cursor.cpuCursorIcon.playerData.Lock();
                cursor.LockCpuCursorIconPosition();
                cursor.ChangeHoldingState(HoldingState.HoldingNothing);
                currentCpuCard = null;
            }

            else if (target.CompareTag("PlayerCard") && target.GetComponent<PlayerCard>().isCpu)
            {
                RemovePlayer(target.GetComponent<PlayerCard>());
            }
        }
    }

    void OnCursorStart(HandCursor cursor)
    {
        if (playerManager.CanStartMatch())
        {
            StartGame();
        }
    }

    void OnCursorCancel(HandCursor cursor)
    {
        if (cursor.holdingState == HoldingState.HoldingNothing)
        {
            cursor.PlayerData.AssignCharacter(null);
            cursor.PlayerData.Unlock();
            cursor.UnlockCursorIconPosition();
            cursor.ChangeHoldingState(HoldingState.HoldingPlayer);
            return;
        }

        
        int numberOfHumanPlayers = 0;
        foreach (var player in playerManager.GetPlayers())
        {
            if (player.PlayerType == PlayerType.Human) numberOfHumanPlayers++;
        }

        Debug.Log($"holding state: {cursor.holdingState}\n human players: {numberOfHumanPlayers} player count: {playerManager.GetPlayers().Count}");

        if (cursor.holdingState == HoldingState.HoldingPlayer && numberOfHumanPlayers == 1)
        {
            Debug.Log("Only 1 human player");
            playerManager.RemovePlayer(cursor.PlayerData);
            SceneManager.LoadScene("MapSelection");
        }

        else if (cursor.holdingState == HoldingState.HoldingPlayer && numberOfHumanPlayers > 1)
        {
            Debug.Log("More than 1 human player");
            playerManager.RemovePlayer(cursor.PlayerData);

        }
    }

    void StartGame()
    {
        gameSetup.GetPlayerData(new List<PlayerData>(playerManager.GetPlayers()));
        SceneManager.LoadScene(gameSetup.map);
    }

    private void RemovePlayer(PlayerCard cpuCard)
    {
        playerManager.RemovePlayer(cpuCard.player);
    }
}
