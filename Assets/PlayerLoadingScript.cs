using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerLoadingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(LobbyManager.instance.IsHosting)
            LobbyManager.instance.changeOwnPlayerVariable("isReady", "y");
        else
        {
            Lobby l = LobbyManager.instance.CurrentLobby;
            string mapData = l.Data["MapData"].Value;
            string mapSizeString = l.Data["MapSize"].Value;
            int mapWidth = Int32.Parse(mapSizeString.Split(',')[0]);
            int mapHeight = Int32.Parse(mapSizeString.Split(',')[1]);
            SetObjects.setMap(GeneticAlgorithmGenerator.multiplayerDataToMap(mapData, mapWidth, mapHeight), false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
