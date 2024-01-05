using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CountDownScript : MonoBehaviour
{
    PlayerManager _playerManagerRefl;
    [SerializeField] BarScoreManager _bsObject;

    // Start is called before the first frame update

    public void startCounting(PlayerManager playerManagerReff)
    {
        _playerManagerRefl = playerManagerReff;
        StartCoroutine(startCountdown());
    }

    public IEnumerator startCountdown()
    {
        Debug.Log("Countdown Start");
        //Time.timeScale = 0;
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
        _playerManagerRefl.setPlayerScriptActive(true);
        _bsObject.StartTimer = true;
        Destroy(gameObject);
    }
}
