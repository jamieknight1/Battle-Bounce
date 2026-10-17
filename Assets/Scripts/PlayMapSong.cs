using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMapSong : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private string mapName;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.StopAll();
    }

    void Start()
    {
        audioManager.PlayAfter($"{mapName} Intro", $"{mapName} Main");
    }
}
