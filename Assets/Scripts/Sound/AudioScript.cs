using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioScript audioObject;

    public Sound[] sounds;

    void Start()
    {
        if (audioObject == null)
            audioObject = this;
        else
        {
            if (GetComponent<AudioSource>().clip == audioObject.GetComponent<AudioSource>().clip)
                Destroy(gameObject);
            else
            {
                Destroy(audioObject.gameObject);
                audioObject = this;
                if (!SoundOnOffManager.isSongOn)
                {
                    Debug.Log("Coba Matiin");
                    audioObject.GetComponent<AudioSource>().playOnAwake = false;
                    audioObject.GetComponent<AudioSource>().Stop();
                }
            }
        }

        DontDestroyOnLoad(gameObject);

    }

    public AudioClip getSound(int id)
    {
        return sounds[id].Audio;
    }

    public AudioClip getSound(string audioName)
    {
        if (SoundOnOffManager.isSFXOn)
            return Array.Find(sounds, hehe => hehe.Name == audioName).Audio;
        return Array.Find(sounds, hehe => hehe.Name == "Null").Audio;
    }
}

[System.Serializable]
public class Sound
{
    public AudioClip Audio;
    public string Name;
}
