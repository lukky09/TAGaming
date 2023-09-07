using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] Slider skipSlider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transitionStart();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            skipSlider.value += Time.deltaTime;
            if (skipSlider.value >= skipSlider.maxValue)
                transitionStart();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
            skipSlider.value = 0;
    }

    void transitionStart()
    {
        if (!PlayerPrefs.HasKey("TutorialDone"))
        {
            PlayerPrefs.SetInt("TutorialDone", 1);
            PlayerPrefs.Save();
            GetComponent<MainMenuNavigation>().changeSceneIndex(-4);
        }
        else
        {
            GetComponent<MainMenuNavigation>().changeSceneIndex(-1);
        }
    }
}
