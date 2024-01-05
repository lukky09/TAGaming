using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerPlacementScript : NetworkBehaviour
{
    int _SpawnID;
    [SerializeField] GameObject Arrow;
    SnowBrawler _snowbrawlerRef;

    // Start is called before the first frame update
    void Start()
    {
        //Matiin script biar g isa gerak
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<ShootMechanic>().enabled = false;
        GetComponent<CatchBall>().enabled = false;
        if(LobbyManager.IsOnline && LobbyManager.instance.IsHosting)
        FindObjectOfType<PlayerManager>().updatePlayerServerRPC();
        _snowbrawlerRef = GetComponent<SnowBrawler>();
        _SpawnID = 1;
        string isLeftTeam = "y";
        if (LobbyManager.IsOnline)
        {
            int thisJoinOrder = 0;
            //Ambil data player ini
            foreach (Player p in LobbyManager.instance.CurrentLobby.Players)
                if (p.Id == LobbyManager.instance.PlayerID)
                {
                    thisJoinOrder = Int32.Parse(p.Data["joinOrder"].Value);
                    isLeftTeam = p.Data["isLeftTeam"].Value;
                    break;
                }
            //Liat berapa order yang lebih kecil dari player
            foreach (Player p in LobbyManager.instance.CurrentLobby.Players)
                if (Int32.Parse(p.Data["joinOrder"].Value) < thisJoinOrder && p.Data["isLeftTeam"].Value.Equals(isLeftTeam))
                    _SpawnID++;
            //Dan juga ganti tim kalau tim kanan
            if (IsOwner)
                updateTeamServerRPC(isLeftTeam.Equals("y"));
        }
        if (FindObjectOfType<SetObjects>() != null)
            transform.position = FindObjectOfType<SetObjects>().GetPositionFromOrderID(_SpawnID, isLeftTeam.Equals("y"));

        //transform.SetParent(GameObject.Find("Players").transform);
        if (IsOwner)
        {
            if (FindObjectOfType<CameraController2D>() != null)
                FindObjectOfType<CameraController2D>().setCameraFollower(gameObject, false);
            Arrow.SetActive(true);
        }
        else
        {
            Destroy(GetComponent<PlayerMovement>());
        }
    }

    [ServerRpc]
    void updateTeamServerRPC(bool playerTeam)
    {
        updateTeam(playerTeam);
        updateTeamClientRPC(playerTeam);
    }

    [ClientRpc]
    void updateTeamClientRPC(bool playerTeam)
    {
        updateTeam(playerTeam);
    }

    void updateTeam(bool playerTeam)
    {
        _snowbrawlerRef.playerteam = playerTeam;
        GetComponent<ColorTaker>().updateColor(playerTeam ? 0 : 1);
        GetComponent<SpriteRenderer>().flipX = !playerTeam;
    }

}
