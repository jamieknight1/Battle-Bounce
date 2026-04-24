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

    PlayerCard cpuCard;

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
            Debug.Log(target.name);
            if (!cursor.PlayerData.IsLocked && target.CompareTag("Player"))
            {
                cursor.PlayerData.AssignCharacter(target.gameObject.GetComponent<CharacterSelectButton>().CharacterPrefab);
                cursor.PlayerData.Lock();
                cursor.LockCursorIconPosition();
                cursor.ChangeHoldingState(HoldingState.HoldingNothing);
            }

            else if (cursor.PlayerData.IsLocked && target.CompareTag("PlayerCard") && cursor.holdingState == HoldingState.HoldingNothing)
            {
                Debug.Log("PlayerCard Hit!");
                if (target.GetComponent<PlayerCard>().cursor == null)
                {
                    cpuCard = playerManager.GetPlayerCards()[playerManager.GetPlayers().Count];
                    cpuCard.isCpu = true;
                    cpuCard.cursor = cursor.gameObject;
                    cpuCard.InitializeHandCursorScript();
                    playerManager.AddCpuPlayer(cursor);
                    cursor.CloseHand();
                    cursor.ChangeHoldingState(HoldingState.HoldingCPU);
                }
            }

            else if (cursor.holdingState == HoldingState.HoldingCPU && target.CompareTag("Player"))
            {
                cpuCard.cursor = null;
                cpuCard.RemoveCursorScript();
                cursor.cpuCursorIcon.playerData.AssignCharacter(target.gameObject.GetComponent<CharacterSelectButton>().CharacterPrefab);
                cursor.cpuCursorIcon.playerData.Lock();
                cursor.LockCpuCursorIconPosition();
                cursor.ChangeHoldingState(HoldingState.HoldingNothing);
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
        cursor.PlayerData.AssignCharacter(null);
        cursor.PlayerData.Unlock();
        cursor.UnlockCursorIconPosition();
        cursor.ChangeHoldingState(HoldingState.HoldingPlayer);
    }

    void StartGame()
    {
        gameSetup.GetPlayerData(new List<PlayerData>(playerManager.GetPlayers()));
        SceneManager.LoadScene(gameSetup.map);
    }
}
