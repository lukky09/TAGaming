using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    bool _startedSearch;
    string _thisPlayerId;

    public Lobby CurrentLobby { get { return _currentLobby; } }
    public bool IsHosting { get { return _isHosting; } }

    public static LobbyManager instance;

    // Start is called before the first frame update
    async void Start()
    {
        _multiplayerManagerRef = transform.GetChild(0).GetComponent<MultiplayerManager>();
        _startedSearch = false;
        _isHosting = false;
        if (instance == null)
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
                Player = getPlayer(true),
                Data = new Dictionary<string, DataObject>
                {
                    {"HasStarted" , new DataObject(DataObject.VisibilityOptions.Public, "n" ) },
                    {"MapSize" , new DataObject(DataObject.VisibilityOptions.Member, "-" ) },
                    {"MapData" , new DataObject(DataObject.VisibilityOptions.Member, "-" ) },
                }
            };
            Lobby multiplayerLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
            _currentLobby = multiplayerLobby;
            _isHosting = true;
            _startButton.interactable = true;

            Debug.Log("Created Lobby with name " + multiplayerLobby.Name + " and max " + multiplayerLobby.MaxPlayers + " players");

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    Player getPlayer(bool isLeftTeam)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _multiplayerManagerRef.MultiplayerName) },
                        {"isLeftTeam", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,(isLeftTeam == true)? "y" : "n") },
                        {"isReady",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"n") }
                    }
        };
    }

    float _heartbeatCooldown = 15;
    float _currentHeartbeatCooldown;
    async void _hostLobbyHeartbeat()
    {
        _currentHeartbeatCooldown -= Time.deltaTime;
        if (_currentHeartbeatCooldown <= 0)
        {
            _currentHeartbeatCooldown = _heartbeatCooldown;
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
        }
    }

    float _updateCooldown = 1.1f;
    float _currentUpdateCooldown;
    async void _LobbyViewUpdate()
    {
        _currentUpdateCooldown -= Time.deltaTime;
        if (_currentUpdateCooldown <= 0)
        {
            _currentUpdateCooldown = _updateCooldown;
            _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
            resetTeams();
        }
        if (!_currentLobby.Data["MapData"].Value.Equals("-"))
        {

            _menuNavigationRef.changeSceneIndex(9);
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
                    Debug.Log(p.Data["isLeftTeam"]);
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

    public async void changeTeam()
    {
        foreach (Player p in _currentLobby.Players)
        {
            if (p.Id == _thisPlayerId)
            {
                Debug.Log(_currentLobby.Id);
                Debug.Log(_thisPlayerId); Debug.Log(p.Data["isLeftTeam"].Value);
                _currentLobby = await LobbyService.Instance.UpdatePlayerAsync(_currentLobby.Id, _thisPlayerId, new UpdatePlayerOptions()
                {
                    Data = new Dictionary<string, PlayerDataObject>{
                        {"isLeftTeam", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,(p.Data["isLeftTeam"].Value == "y")? "n" : "y") }
                    }
                });
            }

        }
    }

    public void startGame()
    {
        changeLobbyVariable("HasStarted","y");
    }

    public async void changeOwnPlayerVariable(string VariableName, string VariableValue)
    {
        _currentLobby = await LobbyService.Instance.UpdatePlayerAsync(_currentLobby.Id, _thisPlayerId, new UpdatePlayerOptions()
        {
            Data = new Dictionary<string, PlayerDataObject>{
                        {VariableName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,VariableValue) }
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

    public async void joinLobby(string lobbbyID, LobbyContentScript lobbyScriptRef)
    {
        try
        {
            JoinLobbyByIdOptions joinOptions = new JoinLobbyByIdOptions()
            {
                Player = getPlayer(lobbyScriptRef.RightTeamPlayerAmount >= lobbyScriptRef.LeftTeamPlayerAmount)
            };
            _currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbbyID, joinOptions);
            _searchScreen.GetComponent<MultiplayerMenuNavigation>().changeScreen(_LobbyScreen);
            _startButton.interactable = false;
            _LobbyViewUpdate();
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
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentLobby != null)
        {
            _LobbyViewUpdate();
            if (_isHosting)
                _hostLobbyHeartbeat();

        }
        else if (_startedSearch)
            searchLobby();
    }
}
