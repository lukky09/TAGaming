using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOnOffManager : MonoBehaviour
{
    public static bool isSongOn = true;
    public static bool isSFXOn = true;

    [SerializeField] bool isSongButton;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;

    private void Start()
    {
        updateImage();
    }

    public void changeSongStatus()
    {
        isSongOn = !isSongOn;
        updateImage();
        if (isSongOn)
            AudioScript.audioObject.GetComponent<AudioSource>().Play();
        else
            AudioScript.audioObject.GetComponent<AudioSource>().Stop();
    }

    public void changeSFXStatus()
    {
        isSFXOn = !isSFXOn;
        updateImage();
    }

    void updateImage()
    {
        if (isSongButton)
        {
            if (isSongOn)
                GetComponent<Image>().sprite = onSprite;
            else
                GetComponent<Image>().sprite = offSprite;
        }
        else
        {
            if (isSFXOn)
                GetComponent<Image>().sprite = onSprite;
            else
                GetComponent<Image>().sprite = offSprite;
        }
    }
}
