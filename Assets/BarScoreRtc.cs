using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BarScoreRtc : NetworkBehaviour
{
    BarScoreManager _barScoreManagerRef;
    int[] _teamScore;

    // Start is called before the first frame update
    void Start()
    {
        _barScoreManagerRef = FindObjectOfType<BarScoreManager>();
        _teamScore = new int[2];
        foreach (SnowBrawler snowbrawler in FindObjectsOfType<SnowBrawler>())
        {
            snowbrawler.BarScoreReference = this;
        }
    }

    [ServerRpc]
    public void addScoreServerRPC(bool forLeftTeam, int amount)
    {
        _teamScore[forLeftTeam ? 0 : 1] += amount;
        updateBarContent();
        updateScoreClientRPC(_teamScore);
    }

    [ClientRpc]
    public void updateScoreClientRPC(int[] teamScore)
    {
        _teamScore = (int[])teamScore.Clone();
        updateBarContent();
    }

    void updateBarContent()
    {
        Debug.Log(_teamScore[0] + "|" + _teamScore[1]);
        _barScoreManagerRef.updateScoreVisuals(_teamScore[0], _teamScore[1]);
    }
}
