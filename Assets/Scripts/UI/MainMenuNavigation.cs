using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNavigation : MonoBehaviour
{
    public void startGame()
    {
        Debug.Log("Nyoom");
        SceneManager.LoadScene("MainLevel");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}