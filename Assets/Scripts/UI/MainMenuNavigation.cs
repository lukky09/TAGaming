using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuNavigation : MonoBehaviour
{
    public static bool isTemplate;
    public static bool startingTransitionActivated = true;
    [SerializeField] GameObject transitionGO;

    public void mainMenuPressed()
    {
        if (!PlayerPrefs.HasKey("DD0"))
            ColorManager.mainGameDefault();
        if (!PlayerPrefs.HasKey("TutorialDone"))
        {
            Debug.Log("Harus Tutorial");
            StartCoroutine(numeratorTransisi(-2));
            return;
        }
        SceneManager.LoadScene(3);
    }

    public static void lastScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public static void goToSettingScene()
    {
        SceneManager.LoadScene("SettingsMenu");
    }


    public void changeSceneIndex(int index)
    {
        StartCoroutine(numeratorTransisi(index));
    }

    public void changeSceneIndexNoTransition(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void nextScene()
    {
        StartCoroutine(numeratorTransisi(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void nextSceneNoTransition()
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

    public void changeTemplate(bool currValue)
    {
        isTemplate = currValue;
    }

    IEnumerator numeratorTransisi(int index)
    {
        if (index < 0)
        {
            startingTransitionActivated = true;
            index = Mathf.Abs(index) - 1;
        }
        if (transitionGO != null)
        {
            transitionGO.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene(index);
    }
}