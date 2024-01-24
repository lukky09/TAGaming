using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BarScoreRtc : NetworkBehaviour
{
    public float TimerCoundown { get { return _timerCountdownServer.Value; } }
    float _timerCountdown;
    int[] _teamScore;

    NetworkVariable<float> _timerCountdownServer = new(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    BarScoreManager _barScoreManagerRef;
    

    // Start is called before the first frame update
    void Start()
    {
        _barScoreManagerRef = FindObjectOfType<BarScoreManager>();
        _teamScore = new int[2];
        foreach (SnowBrawler snowbrawler in FindObjectsOfType<SnowBrawler>())
        {
            snowbrawler.BarScoreReference = this;
        }
        _timerCountdown = 5;
        if (IsServer)
            FindObjectOfType<CountDownScript>().startCounting(GetComponent<PlayerManager>(),5);
        else
            FindObjectOfType<CountDownScript>().startCounting(GetComponent<PlayerManager>(), _timerCountdownServer.Value);
    }

    private void Update()
    {
        if (!IsServer)
            return;
        _timerCountdown -= Time.deltaTime;
        _timerCountdownServer.Value = _timerCountdown;
    }

    [ServerRpc(RequireOwnership = false)]
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
        _barScoreManagerRef.updateScoreVisuals(_teamScore[0], _teamScore[1]);
    }
}
