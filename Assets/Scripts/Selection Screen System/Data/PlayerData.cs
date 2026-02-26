using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{
    public bool IsLocked {get; private set;}
    public PlayerType PlayerType {get; private set;}
    public int PlayerId {get; private set;}
    // public PlayerInput PlayerInput {get; private set;}
    public int DeviceId {get; private set;}
    public GameObject Character {get; private set;}
    public Sprite PreviewCharacter {get; private set;}

    public PlayerData(int playerId, PlayerType playerType, InputDevice device = null)
    {
        PlayerId = playerId;
        PlayerType = playerType;

        if (playerType == PlayerType.Human && device != null)
        {
            DeviceId = device.deviceId;
        }
    }

    public void Lock()
    {
        if (Character == null) return;

        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public void AssignCharacter(GameObject character)
    {
        if (IsLocked) return;

        Character = character;
        //Debug.Log($"character assigned {character}");
    }

    public void SetPlayerId(int id)
    {
        PlayerId = id;
    }
}

public enum PlayerType
{
    Human,
    CPU
}