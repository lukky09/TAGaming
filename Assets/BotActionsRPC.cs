using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BotActionsRPC : SnowbrawlerActionsRPC
{

    private new void Awake()
    {
        base.Awake();
    }


    [ClientRpc]
    public void updateTeamClientRPC(bool isLeftTeam)
    {
        SnowBrawlerRef.playerteam = isLeftTeam;
        GetComponent<ColorTaker>().updateColor(isLeftTeam ? 0 : 1);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
