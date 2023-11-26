using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
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
            string lobbyName = "My Lobby";
            int maxPlayers = 10;
            Unity.Services.Lobbies.Models.Lobby multiplayerLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            Debug.Log("Created Lobby with name " + multiplayerLobby.Name + " and max " + multiplayerLobby.MaxPlayers + " players");

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void searchLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found : " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {

            }
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

    }
}
