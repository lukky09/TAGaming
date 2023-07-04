using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

[IncludeInSettings(true)]
public class BarScoreManager : MonoBehaviour
{
    static protected Slider playerTeamBar;
    static protected Slider enemyTeamBar;
    [SerializeField] int maxScore;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float fightLength;
    [SerializeField] GameObject itemToAnimate;
    [SerializeField] TextMeshProUGUI gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        playerTeamBar = transform.GetChild(1).GetComponent<Slider>();
        enemyTeamBar = transform.GetChild(2).GetComponent<Slider>();
        playerTeamBar.maxValue = maxScore;
        enemyTeamBar.maxValue = maxScore;
        playerTeamBar.value = 0;
        enemyTeamBar.value = 0;
    }

    private void Update()
    {
        fightLength -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(fightLength).ToString();
        if ((fightLength <= 0|| playerTeamBar.value>= maxScore || enemyTeamBar.value>= maxScore) && itemToAnimate !=null)
            StartCoroutine(victoryAnimation());
    }

    public static void addscore(bool forPlayerTeam, float amount)
    {
        if (playerTeamBar == null)
            return;
        if (forPlayerTeam)
            playerTeamBar.value += amount;
        else
            enemyTeamBar.value += amount;
    }

    IEnumerator victoryAnimation()
    {
        Time.timeScale = 0;
        itemToAnimate?.SetActive(true);
        if(fightLength <= 0)
         gameOverText.text = "Time Up!";
        else
            gameOverText.text = "Game Set!";
        yield return new WaitForSecondsRealtime(4);
        if (playerTeamBar.value >= enemyTeamBar.value)
            gameOverText.text = "Player Team wins";
        else if(enemyTeamBar.value >= playerTeamBar.value)
            gameOverText.text = "enemy team wins";
        yield return new WaitForSecondsRealtime(2);
        MainMenuNavigation.changeSceneIndex(0);
    }
}
