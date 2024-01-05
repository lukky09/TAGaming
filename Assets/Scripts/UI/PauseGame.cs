using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseScreen;

    public static bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                pausegame(false);
            else
                pausegame(true);
        }
    }

    void pausegame(bool pause)
    {
        isPaused = pause;
        pauseScreen.SetActive(pause);
        Time.timeScale = 1 - Convert.ToInt32(pause);
    }

    public void continueGame()
    {
        pausegame(false);
    }

    public void exitLevel()
    {
        NetworkManager.Singleton.Shutdown();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
