using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    public static bool mandatoryTutorial = false;
    [SerializeField] Slider skipSlider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(transitionStart());
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            skipSlider.value += Time.deltaTime;
            if (skipSlider.value >= skipSlider.maxValue)
                StartCoroutine(transitionStart());
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
            skipSlider.value = 0;
    }

    IEnumerator transitionStart()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1f);
        PlayerPrefs.SetString("TutorialDone", "Yes");
        PlayerPrefs.Save();
        Time.timeScale = 1;
        if (mandatoryTutorial)
        {
            PlayerPrefs.SetInt("TutorialDone", 1);
            PlayerPrefs.Save();
            Debug.Log("Tutorial Wajib mari");
            mandatoryTutorial = false;
            GetComponent<MainMenuNavigation>().changeSceneIndex(-4);
        }
        GetComponent<MainMenuNavigation>().changeSceneIndex(-1);


    }
}
