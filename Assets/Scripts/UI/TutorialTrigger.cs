using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    public static bool mandatoryTutorial = false;
    [SerializeField] GameObject transition;
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
        transition.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        PlayerPrefs.SetString("TutorialDone", "Yes");
        PlayerPrefs.Save();
        if (mandatoryTutorial)
        {
            mandatoryTutorial = false;
            MainMenuNavigation.changeSceneIndex(3);
        }
        MainMenuNavigation.changeSceneIndex(0);


    }
}
