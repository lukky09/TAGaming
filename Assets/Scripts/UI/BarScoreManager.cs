using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using Unity.Netcode;

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
    [SerializeField] TextMeshProUGUI gameOverText;
     PlayerManager playerManagerRef;
    public bool StartTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        Transform uiCanvas = FindObjectOfType<Canvas>().transform;
        playerTeamBar = uiCanvas.GetChild(1).GetComponent<Slider>();
        textLeft = uiCanvas.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        enemyTeamBar = uiCanvas.GetChild(2).GetComponent<Slider>();
        textRight = uiCanvas.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
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
        timerText.text = Mathf.CeilToInt(fightLength).ToString().PadLeft(3, '0');
        if ((fightLength <= 0 || playerTeamBar.value >= maxScore || enemyTeamBar.value >= maxScore) && itemToAnimate != null)
            StartCoroutine(victoryAnimation());
    }

    public void updateScoreVisuals(int leftTeamAmount, int rightTeamAmount)
    {
        if (playerTeamBar == null)
            return;
        playerTeamBar.value = leftTeamAmount;
        enemyTeamBar.value = rightTeamAmount;
        textLeft.text = playerTeamBar.value.ToString();
        textRight.text = enemyTeamBar.value.ToString();
    }

    IEnumerator victoryAnimation()
    {
        StartTimer = false;
        playerManagerRef = FindObjectOfType<PlayerManager>();
        playerManagerRef.setPlayerScriptActive(false);
        itemToAnimate?.SetActive(true);
        if (fightLength <= 0)
            gameOverText.text = "Time Up!";
        else
            gameOverText.text = "Game Set!";
        yield return new WaitForSecondsRealtime(4);
        if (playerTeamBar.value >= enemyTeamBar.value)
            gameOverText.text = "Player Team wins";
        else if (enemyTeamBar.value >= playerTeamBar.value)
            gameOverText.text = "enemy team wins";
        playerManagerRef.gameOverAnimation(playerTeamBar.value >= enemyTeamBar.value);
        yield return new WaitForSecondsRealtime(2);
        NetworkManager.Singleton.Shutdown();
        GetComponent<MainMenuNavigation>().changeSceneIndex(-1);
    }
}
