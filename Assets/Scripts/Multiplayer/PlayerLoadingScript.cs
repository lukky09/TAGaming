using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoadingScript : MonoBehaviour
{
    Lobby _lobby;
    // Start is called before the first frame update
    void Start()
    {
        _lobby = LobbyManager.instance.CurrentLobby;
        if (LobbyManager.instance.IsHosting)
            LobbyManager.instance.changeOwnPlayerVariable("isReady", "y");
        else
        {
            string mapData = _lobby.Data["MapData"].Value;
            string mapSizeString = _lobby.Data["MapSize"].Value;
            int mapWidth = Int32.Parse(mapSizeString.Split(',')[0]);
            int mapHeight = Int32.Parse(mapSizeString.Split(',')[1]);
            SetObjects.setMap(GeneticAlgorithmGenerator.multiplayerDataToMap(mapData, mapWidth, mapHeight), false);
            LobbyManager.instance.changeOwnPlayerVariable("isReady", "y");
            Debug.Log("Map Siap");
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool _everyoneReady = true;
        foreach (Player p in _lobby.Players)
        {
            if (!p.Data["isReady"].Value.Equals("y"))
            {
                Debug.Log(p.Data["Name"].Value+" punya nilai "+ p.Data["isReady"].Value);
                _everyoneReady = false;
                break;
            }
        }
        if (_everyoneReady)
            SceneManager.LoadScene(5);
    }
}
