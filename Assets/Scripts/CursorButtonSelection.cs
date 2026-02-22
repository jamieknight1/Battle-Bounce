using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorButtonSelection : MonoBehaviour
{

    [SerializeField] public string scene;
    [SerializeField] public string mapDescription;
    [SerializeField] GameSetup gameSetup;

    public void OnClick()
    {
        gameSetup.map = scene;
        //selectionScreenInputManager.selectedScene = scene;
        SceneManager.LoadScene("CharacterSelect");
    }
}
