using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenButtons : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Game Setup Screen");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
