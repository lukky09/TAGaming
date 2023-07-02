using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuNavigation : MonoBehaviour
{
    public static void mainMenuPressed()
    {
        if (!PlayerPrefs.HasKey("DD0"))
            ColorManager.mainGameDefault();
        if (!PlayerPrefs.HasKey("TutorialDone")) {
            TutorialTrigger.mandatoryTutorial = true;
            SceneManager.LoadScene(1);
        }
        SceneManager.LoadScene(3);
    }

    public static void lastScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public static void changeSceneIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public static void goToSettingScene()
    {
        SceneManager.LoadScene("SettingsMenu");
    }

    public static void nextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void exitGame()
    {
        Application.Quit();
    }

    public void setLevelforUser(string preparsedint)
    {
        string[] split = preparsedint.Split(","[0]);
        int width = int.Parse(split[0]) + 2;
        int height = int.Parse(split[1]) + 2;
        SetObjects.initializeSize(width, height);
        nextScene();
        //SceneManager.LoadScene("MainLevel");
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