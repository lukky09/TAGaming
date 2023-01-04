using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuNavigation : MonoBehaviour
{
    public void nextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void startGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void exitGame()
    {
        Application.Quit();
    }

    //Untuk Pengecekan
    public void makeInputEven(string input)
    {
        int intinput = int.Parse(input);
        if (intinput % 2 == 1)
        {
            gameObject.GetComponent<TMP_InputField>().text = (intinput - 1).ToString();
        }
    }
}