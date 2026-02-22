using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{
    public bool IsLocked {get; private set;}
    public PlayerType PlayerType {get; private set;}
    public int PlayerId {get; private set;}
    public InputDevice PlayerInput {get; private set;}
    public GameObject Character {get; private set;}

    public void IsCpu(bool isCpu)
    {
        if (isCpu) {PlayerType = PlayerType.CPU;}
        if (!isCpu) {PlayerType = PlayerType.Human;}
    }

    public void Selected(bool selected)
    {
        if (selected) {IsLocked = true;}
        if (!selected) {IsLocked = false;}
    }

    public void AssignPlayerId(int id)
    {
        PlayerId = id;
    }

    public void AssignInput(InputDevice input)
    {
        if (PlayerType == PlayerType.CPU) {return;}
        else if (PlayerType == PlayerType.Human) {PlayerInput = input;}
    }

    public void AssignCharacter(GameObject character)
    {
        Character = character;
    }
}

public enum PlayerType
{
    Human,
    CPU
}

//NOTES
//
//create a scriptable object for playerdata and reference it in this script
//look at chatgpt