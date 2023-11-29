using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject _lobbyScrollViewViewport;
    [SerializeField] GameObject _scrollViewContentPrefab;
    [SerializeField] MultiplayerManager _multiplayerManagerRef;
    [SerializeField] GameObject _LobbyScreen;
    [SerializeField] GameObject _searchScreen;
    Lobby _hostLobby;
    bool _startedSearch;

    // Start is called before the first frame update
    async void Start()
    {
        _multiplayerManagerRef = transform.GetChild(0).GetComponent<MultiplayerManager>();
        _startedSearch = false;

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void createLobby()
    {
        try
        {
            string lobbyName = _multiplayerManagerRef.MultiplayerName + "'s Lobby";
            int maxPlayers = 10;
            Lobby multiplayerLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            _hostLobby = multiplayerLobby;

            Debug.Log("Created Lobby with name " + multiplayerLobby.Name + " and max " + multiplayerLobby.MaxPlayers + " players");

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    float _heartbeatCooldown = 15;
    float _currentHeartbeatCooldown;
    async void _hostLobbyHeartbeat()
    {
        _currentHeartbeatCooldown -= Time.deltaTime;
        if (_currentHeartbeatCooldown <= 0)
        {
            _currentHeartbeatCooldown = _heartbeatCooldown;
            await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
        }
    }

    public async void searchLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found : " + queryResponse.Results.Count);
            GameObject scView;
            int i = 0;
            foreach (Transform item in _lobbyScrollViewViewport.transform)
            {
                Destroy(item.gameObject);
            }
            foreach (Lobby lobby in queryResponse.Results)
            {
                scView = Instantiate(_scrollViewContentPrefab);
                scView.GetComponent<LobbyContentScript>().initialize(lobby.Name, lobby.Players.Count, lobby.Id, this);
                scView.transform.SetParent(_lobbyScrollViewViewport.transform);
                scView.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                scView.GetComponent<RectTransform>().offsetMax = new Vector2(0, -150 * i);
                scView.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 150);
                i++;
                Debug.Log(scView.name);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    public async void joinLobby(string lobbbyID)
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByIdAsync(lobbbyID);
            _searchScreen.GetComponent<MultiplayerMenuNavigation>().changeScreen(_LobbyScreen);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_hostLobby != null)
            _hostLobbyHeartbeat();
        else if (_startedSearch)
            searchLobby();
    }
}
