using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;
        }
    }

    void Start()
    {
        PlayAfter("Title Screen Intro", "Title Screen Main");
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            Debug.Log($"Sound {name} not found!");
            return;
        }
        sound.source.Play();
    }

    public void PlayAfter(string firstSoundName, string secondSoundName)
    {
        Sound firstSound = Array.Find(sounds, sound => sound.name == firstSoundName);
        if (firstSound == null)
        {
            Debug.Log($"Sound {name} not found!");
        }

        Sound secondSound = Array.Find(sounds, sound => sound.name == secondSoundName);
        if (secondSound == null)
        {
            Debug.Log($"Sound {name} not found!");
        }
        
        double startTime = AudioSettings.dspTime + 1;
        double firstSoundLength = (double)firstSound.clip.samples/firstSound.clip.frequency;
        firstSound.source.PlayScheduled(startTime);
        secondSound.source.PlayScheduled(startTime + firstSoundLength);
    }

    public void StopAll()
    {
        foreach (var sound in sounds)
        {
            sound.source.Stop();
        }
    }
}
