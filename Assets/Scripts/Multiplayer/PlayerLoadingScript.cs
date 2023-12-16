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
    // Start is called before the first frame update
    void Start()
    {
        if (LobbyManager.instance.IsHosting)
        {
            Debug.Log("Hosting");
            LobbyManager.instance.changeOwnPlayerVariable("isReady", PlayerDataObject.VisibilityOptions.Member, "y");
        }
        else
        {
            string mapData = _lobby.Data["MapData"].Value;
            string mapSizeString = _lobby.Data["MapSize"].Value;
            int mapWidth = Int32.Parse(mapSizeString.Split(',')[0]);
            int mapHeight = Int32.Parse(mapSizeString.Split(',')[1]);
            SetObjects.setMap(GeneticAlgorithmGenerator.multiplayerDataToMap(mapData, mapWidth, mapHeight), false);
            LobbyManager.instance.changeOwnPlayerVariable("isReady", PlayerDataObject.VisibilityOptions.Member, "y");
            Debug.Log("Map Siap");
        }
    }

    float currentLobbyUpdateTimer = 0;
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
            //Debug.Log(p.Data["Name"].Value + " "+p.Data["isReady"].Value);
            if (!p.Data["isReady"].Value.Equals("y"))
            {
                _everyoneReady = false;
                break;
            }
        }
        if (_everyoneReady)
            SceneManager.LoadScene(5);
    }
}
