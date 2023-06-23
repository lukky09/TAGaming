using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] GameObject transition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(transitionStart());
        }
    }

    IEnumerator transitionStart()
    {
        Time.timeScale = 0;
        transition.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        MainMenuNavigation.nextScene();
    }
}
