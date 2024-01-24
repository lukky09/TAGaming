using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoadingScript : MonoBehaviour
{
    Lobby _lobby { get { return LobbyManager.instance.CurrentLobby; } }
    RelayManager _relayManRef;
    // Start is called before the first frame update
    async void Start()
    {
        _relayManRef = GetComponent<RelayManager>();
        if (LobbyManager.instance.IsHosting)
        {
            LobbyManager.instance.changeOwnPlayerVariable("isReady", PlayerDataObject.VisibilityOptions.Member, "y");
            string joinRelayCode = await _relayManRef.CreateRelay();
            LobbyManager.instance.changeLobbyVariable("RelayCode", DataObject.VisibilityOptions.Member, joinRelayCode);
        }
        else
        {
            string mapData = _lobby.Data["MapData"].Value;
            string mapSizeString = _lobby.Data["MapSize"].Value;
            int mapWidth = Int32.Parse(mapSizeString.Split(',')[0]);
            int mapHeight = Int32.Parse(mapSizeString.Split(',')[1]);
            SetObjects.setMap(GeneticAlgorithmGenerator.multiplayerDataToMap(mapData, mapWidth, mapHeight), true);
            LobbyManager.instance.changeOwnPlayerVariable("isReady", PlayerDataObject.VisibilityOptions.Member, "y");
        }
    }

    float currentLobbyUpdateTimer = 1;
    // Update is called once per frame
    void Update()
    {
        currentLobbyUpdateTimer -= Time.deltaTime;
        if (LobbyManager.instance.IsHosting)
            LobbyManager.instance.hostLobbyHeartbeat();
        if (currentLobbyUpdateTimer <= 0)
        {
            LobbyManager.instance.updateLobby();
            currentLobbyUpdateTimer = 1.1f;
        }
        bool _everyoneReady = true;
        foreach (Player p in _lobby.Players)
        {
            if (!p.Data["isReady"].Value.Equals("y"))
            {
                _everyoneReady = false;
                break;
            }
        }
        if (_everyoneReady && !_lobby.Data["RelayCode"].Value.Equals("0"))
        {
            if (!LobbyManager.instance.IsHosting)
                _relayManRef.JoinRelay(_lobby.Data["RelayCode"].Value);
            SceneManager.LoadScene(5);
        }
    }
}
