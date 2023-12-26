using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject _lobbyScrollViewViewport;
    [SerializeField] GameObject _scrollViewContentPrefab;
    [SerializeField] MultiplayerManager _multiplayerManagerRef;
    [SerializeField] GameObject _LobbyScreen;
    [SerializeField] GameObject _searchScreen;
    [SerializeField] TextMeshProUGUI _team1Text;
    [SerializeField] TextMeshProUGUI _team2Text;
    [SerializeField] Button _changeTeamButton;
    [SerializeField] Button _startButton;
    [SerializeField] MainMenuNavigation _menuNavigationRef;
    Lobby _currentLobby;
    bool _isHosting;
    bool _isOnline;
    bool _startedSearch;
    string _thisPlayerId;

    public Lobby CurrentLobby { get { return _currentLobby; } }
    public bool IsHosting { get { return _isHosting; } }
    public bool IsOnline { get { return _isOnline; } }
    public string PlayerID { get { return _thisPlayerId; } }

    public static LobbyManager instance;

    // Start is called before the first frame update
    async void Start()
    {
        _isOnline = false;
        _multiplayerManagerRef = transform.GetChild(0).GetComponent<MultiplayerManager>();
        _startedSearch = false;
        _isHosting = false;
        instance = this;

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
            _thisPlayerId = AuthenticationService.Instance.PlayerId;
        };
        AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void createLobby()
    {
        try
        {
            string lobbyName = _multiplayerManagerRef.MultiplayerName + "'s Lobby";
            int maxPlayers = 10;
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                Player = getPlayer(true, 0),
                Data = new Dictionary<string, DataObject>
                {
                    {"HasStarted" , new DataObject(DataObject.VisibilityOptions.Public, "n" ) },
                    {"MapSize" , new DataObject(DataObject.VisibilityOptions.Member, "-" ) },
                    {"MapData" , new DataObject(DataObject.VisibilityOptions.Member, "-" ) },
                    {"PlayerOrder" , new DataObject(DataObject.VisibilityOptions.Member, createPlayerOrder() ) }
                }
            };
            Lobby multiplayerLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
            _currentLobby = multiplayerLobby;
            _isHosting = true;
            _startButton.interactable = true;
            _isOnline = true;

            Debug.Log("Created Lobby with name " + multiplayerLobby.Name + " and max " + multiplayerLobby.MaxPlayers + " players");

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    Player getPlayer(bool isLeftTeam, int order)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _multiplayerManagerRef.MultiplayerName) },
                        {"isLeftTeam", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,(isLeftTeam == true)? "y" : "n") },
                        {"isReady",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"n") },
                        {"joinOrder",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,$"{order}") }
                    }
        };
    }

    float _heartbeatCooldown = 15;
    float _currentHeartbeatCooldown;
    public async void hostLobbyHeartbeat()
    {
        _currentHeartbeatCooldown -= Time.deltaTime;

        if (_currentHeartbeatCooldown <= 0)
        {
            UnityEngine.Debug.Log("Hearbeat");
            _currentHeartbeatCooldown = _heartbeatCooldown;
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
        }
    }

    float _updateCooldown = 1.1f;
    float _currentUpdateCooldown;
    async void LobbyViewUpdate()
    {
        _currentUpdateCooldown -= Time.deltaTime;
        if (_currentUpdateCooldown <= 0)
        {
            _currentUpdateCooldown = _updateCooldown;
            updateLobby();
            resetTeams();
        }
        if (!_currentLobby.Data["MapData"].Value.Equals("-"))
        {
            _menuNavigationRef.changeSceneIndexNoTransition(7);
        }
    }

    public async void searchLobby()
    {
        try
        {
            resetLobbySearch();
            Debug.Log("Cari Lobby");
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found : " + queryResponse.Results.Count);
            GameObject scView;
            int i = 0, leftPlayerAmt, rightPlayerAmt;
            foreach (Lobby lobby in queryResponse.Results)
            {
                if (!lobby.Data["HasStarted"].Value.ToString().Equals("n"))
                    continue;
                scView = Instantiate(_scrollViewContentPrefab);
                leftPlayerAmt = 0;
                rightPlayerAmt = 0;
                foreach (Player p in lobby.Players)
                {
                    if (p.Data.ContainsKey("isLeftTeam") && p.Data["isLeftTeam"].Value.Equals("y"))
                        leftPlayerAmt += 1;
                    else
                        rightPlayerAmt += 1;
                }
                scView.GetComponent<LobbyContentScript>().initialize(lobby.Name, leftPlayerAmt, rightPlayerAmt, lobby.Id, this);
                scView.transform.SetParent(_lobbyScrollViewViewport.transform);
                scView.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                scView.GetComponent<RectTransform>().offsetMax = new Vector2(0, -150 * i);
                scView.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 150);
                i++;

            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    public async void updateLobby()
    {
        _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
    }

    void resetLobbySearch()
    {
        foreach (Transform item in _lobbyScrollViewViewport.transform)
        {
            Destroy(item.gameObject);
        }
    }
    void resetTeams()
    {
        _team1Text.text = string.Empty;
        _team2Text.text = string.Empty;
        string team1 = "";
        string team2 = "";
        int team1Amt = 0;
        int team2Amt = 0;
        Player you = null;
        foreach (Player p in _currentLobby.Players)
        {

            if (p.Data["isLeftTeam"].Value.Equals("y"))
            {
                team1 += p.Data["Name"].Value + "\n";
                team1Amt += 1;
            }
            else
            {
                team2 += p.Data["Name"].Value + "\n";
                team2Amt += 1;
            }
            if (p.Id == _thisPlayerId)
                you = p;
        }
        _team1Text.text = team1;
        _team2Text.text = team2;
        if ((you.Data["isLeftTeam"].Value.Equals("y") && team2Amt == 5) || (you.Data["isLeftTeam"].Value.Equals("n") && team1Amt == 5))
            _changeTeamButton.interactable = false;
        else
            _changeTeamButton.interactable = true;

    }

    public void changeTeam()
    {
        foreach (Player p in _currentLobby.Players)
        {
            if (p.Id == _thisPlayerId)
            {
                changeOwnPlayerVariable("isLeftTeam", PlayerDataObject.VisibilityOptions.Public, (p.Data["isLeftTeam"].Value == "y") ? "n" : "y");
            }

        }
    }
    
    public void startGame()
    {
        changeLobbyVariable("HasStarted", "y");
    }

    public async void changeOwnPlayerVariable(string VariableName, PlayerDataObject.VisibilityOptions visibility, string VariableValue)
    {
        _currentLobby = await LobbyService.Instance.UpdatePlayerAsync(_currentLobby.Id, _thisPlayerId, new UpdatePlayerOptions()
        {
            Data = new Dictionary<string, PlayerDataObject>{
                        {VariableName, new PlayerDataObject(visibility,VariableValue) }
                    }
        });
    }

    public async void changeLobbyVariable(string VariableName, string VariableValue)
    {
        _currentLobby = await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions()
        {
            Data = new Dictionary<string, DataObject>
            {
                {VariableName, new DataObject(DataObject.VisibilityOptions.Public,VariableValue)  }
            }
        });
    }

    public async void changeLobbyVariable(string[] VariableName, string[] VariableValue)
    {
        Dictionary<string, DataObject> d = new Dictionary<string, DataObject>();
        for (int i = 0; i < VariableName.Length; i++)
        {
            d.Add(VariableName[i], new DataObject(DataObject.VisibilityOptions.Member, VariableValue[i]));
        }
        _currentLobby = await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions()
        {
            Data = d
        });
    }

    string createPlayerOrder()
    {
        int[,] teamOrder = new int[,] { { 1, 2, 3, 4, 5 }, { 1, 2, 3, 4, 5 } };
        int numberStorage, randomedIndex;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                randomedIndex = UnityEngine.Random.Range(0,5);
                numberStorage = teamOrder[i, j];
                teamOrder[i, j] = teamOrder[i, randomedIndex];
                teamOrder[i, randomedIndex] = numberStorage;
            }
        }
        string result = string.Empty;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                result += teamOrder[i, j].ToString();
            }
            result += ",";
        }
        result.Remove(result.Length - 1);
        return result;
    }

        public async void joinLobby(string lobbbyID)
    {
        try
        {
            Lobby seenLobby = await Lobbies.Instance.GetLobbyAsync(lobbbyID);
            int jumKiri = 0, jumKanan = 0, orderTertinggi = 0;
            foreach (Player p in seenLobby.Players)
            {
                if (p.Data["isLeftTeam"].Value.Equals("y"))
                    jumKiri++;
                else
                    jumKanan++;
                if (Int32.Parse(p.Data["joinOrder"].Value) > orderTertinggi)
                    orderTertinggi = Int32.Parse(p.Data["joinOrder"].Value);
            }
            JoinLobbyByIdOptions joinOptions = new JoinLobbyByIdOptions()
            {
                Player = getPlayer(jumKiri <= jumKanan, orderTertinggi + 1)
            };
            _currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbbyID, joinOptions);
            _searchScreen.GetComponent<MultiplayerMenuNavigation>().changeScreen(_LobbyScreen);
            _startButton.interactable = false;
            LobbyViewUpdate();
            _isOnline = true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    public async void quitLobby()
    {
        if (_isHosting)
            if (_currentLobby.Players.Count > 1)
                foreach (Player p in _currentLobby.Players)
                    if (p.Id != _thisPlayerId)
                        await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions()
                        {
                            HostId = p.Id
                        });
                    else
                        await LobbyService.Instance.DeleteLobbyAsync(_currentLobby.Id);
            else
                await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, _thisPlayerId);
        _currentLobby = null;
        _isOnline = false;
        _isHosting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentLobby != null)
        {
            LobbyViewUpdate();
            if (_isHosting)
                hostLobbyHeartbeat();

        }
        else if (_startedSearch)
            searchLobby();
    }
}
