using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEditor.Progress;

public class MainMenuNavigation : MonoBehaviour
{
    public void nextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void setLevelforUser(string preparsedint)
    {
        string[] split = preparsedint.Split(","[0]);
        int width = int.Parse(split[0]);
        int height = int.Parse(split[1]);
        SetStones.initializeSize(width, height);
        SceneManager.LoadScene("MainLevel");
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