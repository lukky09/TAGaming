using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowBrawler : MonoBehaviour
{
    static protected Slider playerTeamBar;
    static protected Slider enemyTeamBar;

    // Start is called before the first frame update
    static protected void initialize()
    {
        GameObject temp = GameObject.Find("UICanvas");
        playerTeamBar = temp.transform.GetChild(1).GetComponent<Slider>();
        enemyTeamBar = temp.transform.GetChild(2).GetComponent<Slider>();
        playerTeamBar.value = 0;
        enemyTeamBar.value = 0;
    }

    protected void addscore(bool forPlayerTeam,float amount)
    {
        if (forPlayerTeam)
            playerTeamBar.value += amount;
        else
            enemyTeamBar.value += amount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
