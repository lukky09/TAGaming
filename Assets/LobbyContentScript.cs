using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class LobbyContentScript : MonoBehaviour
{
    TextMeshProUGUI _lobbyNameText;
    TextMeshProUGUI _playerAmountText;
    string _lobbyID;
    Button _joinButton;
    LobbyManager _lobbyManagerRef;

    public string LobbyName { set { _lobbyNameText.text = value; } }
    public int PlayerAmount { set { _playerAmountText.text = value.ToString() + "/10"; } }
    public string LobbyID { get { return _lobbyID; } set { _lobbyID = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        _lobbyNameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _playerAmountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _joinButton = transform.GetChild(2).GetComponent<Button>();
        Debug.Log(_lobbyNameText.text);
    }

    public void initialize(string lobbyName, int playerAmount, string lobbyID,LobbyManager lobbyManager)
    {
        LobbyName = lobbyName;
        PlayerAmount = playerAmount;
        LobbyID = lobbyID;
        _lobbyManagerRef = lobbyManager; 
        if (playerAmount == 10)
        {
            _joinButton.interactable = false;
            _joinButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "full";
        }
        else
        {
            _joinButton.interactable = true;
            _joinButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "join";
        }
    }

    public void joinButtonClicked()
    {
        try
        {
            _lobbyManagerRef.joinLobby(LobbyID);
            Debug.Log("Room Joined");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
       
    }

}
