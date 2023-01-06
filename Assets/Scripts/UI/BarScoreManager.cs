using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

[IncludeInSettings(true)]
public class BarScoreManager : MonoBehaviour
{
    static protected Slider playerTeamBar;
    static protected Slider enemyTeamBar;
    // Start is called before the first frame update
    void Start()
    {
        playerTeamBar = transform.GetChild(1).GetComponent<Slider>();
        enemyTeamBar = transform.GetChild(2).GetComponent<Slider>();
        playerTeamBar.value = 0;
        enemyTeamBar.value = 0;
    }

    public static void addscore(bool forPlayerTeam, float amount)
    {
        if (forPlayerTeam)
            playerTeamBar.value += amount;
        else
            enemyTeamBar.value += amount;
    }
}
