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
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] int fightLength;
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

    public static void addscore(bool forPlayerTeam, float amount)
    {
        if (playerTeamBar == null)
            return;
        if (forPlayerTeam)
            playerTeamBar.value += amount;
        else
            enemyTeamBar.value += amount;
    }
}
