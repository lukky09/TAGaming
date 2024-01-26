using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlacementScript : NetworkBehaviour
{
    int _SpawnID;
    [SerializeField] GameObject Arrow;
    [SerializeField] GameObject Text;
    SnowBrawler _snowbrawlerRef;

    NetworkVariable<FixedString64Bytes> _characterName = new("Dunno", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
            _characterName.Value = NameCheck.MultiplayerName;
        //Nyalain Script kalau tutorial
        if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<ShootMechanic>().enabled = true;
            GetComponent<CatchBall>().enabled = true;
        }
        if (LobbyManager.IsOnline && LobbyManager.instance.IsHosting)
            FindObjectOfType<PlayerManager>().doPlayersUpdate();
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
            //Ini dilakukan agar karakter di server bisa mengambil bola
            _snowbrawlerRef.SnowballManagerRef = FindObjectOfType<SnowBallManager>();
            Destroy(GetComponent<PlayerMovement>());
            Text.SetActive(true);
            Text.GetComponent<TextMeshProUGUI>().text = _characterName.Value.ToString();
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
        transform.localScale = new Vector3(playerTeam ? 1 : -1, 1, 1);
    }

}
