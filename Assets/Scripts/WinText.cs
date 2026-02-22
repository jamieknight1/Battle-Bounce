using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WinText : MonoBehaviour
{
    TMP_Text text;
    [SerializeField] GameSetup gameSetup;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        text.text = "Player " + gameSetup.winningPlayerNumber.ToString() + " Wins!";
    }
}
