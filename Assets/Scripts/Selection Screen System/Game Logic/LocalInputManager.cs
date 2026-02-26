using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class LocalInputManager : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] PlayerInputManager playerInputManager;

    void OnEnable()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    void OnDisable()
    {
        playerInputManager.onPlayerJoined -= OnPlayerJoined;
        playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        playerManager.AddHumanPlayer(playerInput.devices[0], playerInput.gameObject);
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        PlayerData playerToRemove = null;
        foreach (var player in playerManager.GetPlayers())
        {
            if (player.DeviceId == playerInput.devices[0].deviceId)
            {
                playerToRemove = player;
                break;
            }
        }

        if (playerToRemove != null)
        {
            playerManager.RemovePlayer(playerToRemove);
            playerManager.UpdatePlayerId();
        }
    }
}
