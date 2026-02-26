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
        if (target != null && !cursor.PlayerData.IsLocked)
        {
            cursor.PlayerData.AssignCharacter(target.gameObject.GetComponent<CharacterSelectButton>().CharacterPrefab);
            cursor.PlayerData.Lock();
            cursor.LockCursorIconPosition();
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
    }

    void StartGame()
    {
        gameSetup.GetPlayerData(new List<PlayerData>(playerManager.GetPlayers()));
        //Debug.Log($"player count before starting game: {playerManager.GetPlayers().Count}");
        SceneManager.LoadScene(gameSetup.map);
    }
}
