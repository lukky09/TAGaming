using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CountDownScript : MonoBehaviour
{
    [SerializeField] PlayersManager _pmObject;
    [SerializeField] BarScoreManager _bsObject;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startCountdown());
    }

    IEnumerator startCountdown()
    {
        //Time.timeScale = 0;
        _pmObject.activatePlayersScript(false);
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        // Tunggu animasi transisi mari
        yield return new WaitForSecondsRealtime(1);
        text.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            text.text = i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }
        text.text = "GO!";
        yield return new WaitForSecondsRealtime(1);
        //Time.timeScale = 1;
        _pmObject.activatePlayersScript(true);
        _bsObject.StartTimer = true;
        Destroy(gameObject);
    }
}
