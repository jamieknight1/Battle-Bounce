using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepAlive : MonoBehaviour
{
    private GameObject sceneManager;

    private void Awake()
    {
        if (sceneManager == null)
        {
            sceneManager = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        // else
        //     Destroy(gameObject);
    }

    // void Update()
    // {
    //     if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
    //     {
    //         Destroy(gameObject);
    //     }

    //     if (SceneManager.GetActiveScene().name == "CharacterSelect")
    //     {
    //         GetComponent<GamepadJoinBehavior>().enabled = true;
    //     }
    // }
}
