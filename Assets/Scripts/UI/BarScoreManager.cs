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
    static protected TextMeshProUGUI textLeft;
    static protected TextMeshProUGUI textRight;
    [SerializeField] int maxScore;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float fightLength;
    [SerializeField] GameObject itemToAnimate;
    [SerializeField] GameObject transition;
    [SerializeField] TextMeshProUGUI gameOverText;
    public bool StartTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTeamBar = transform.GetChild(1).GetComponent<Slider>();
        textLeft = transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        enemyTeamBar = transform.GetChild(2).GetComponent<Slider>();
        textRight = transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        playerTeamBar.maxValue = maxScore;
        enemyTeamBar.maxValue = maxScore;
        playerTeamBar.value = 0;
        enemyTeamBar.value = 0;
        textLeft.text = "0";
        textRight.text = "0";
        timerText.text = Mathf.CeilToInt(fightLength).ToString().PadLeft(3, '0');
    }

    private void Update()
    {
        if (!StartTimer)
            return;
        fightLength -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(fightLength).ToString().PadLeft(3,'0');
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
        updateScoreText();
    }

    static void updateScoreText()
    {
        textLeft.text = playerTeamBar.value.ToString();
        textRight.text = enemyTeamBar.value.ToString();
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
        Time.timeScale = 1;
        GetComponent<MainMenuNavigation>().changeSceneIndex(-1);
    }
}
