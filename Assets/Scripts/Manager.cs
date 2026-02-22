using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
    public GameObject cursorpf;

    public void OnPlayerJoin(PlayerInput pi)
    {
        Debug.Log(pi.playerIndex + 1);
        Debug.Log(pi.currentControlScheme);
        GameObject cursor = Instantiate(cursorpf);
        cursor.SetActive(false);
    }

    public void OnReady()
    {

    }
}
