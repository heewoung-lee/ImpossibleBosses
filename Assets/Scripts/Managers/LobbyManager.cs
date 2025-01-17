
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public struct PlayerIngameLoginInfo
{
    private readonly string _nickname;
    private readonly string _Id;
    public string PlayerNickName => _nickname;
    public string Id => _Id;

    public PlayerIngameLoginInfo(string playerNickname, string playerId)
    {
        _nickname = playerNickname; // ��� �ʵ带 �ʱ�ȭ
        _Id = playerId;
    }
}


public class LobbyManager
{
    PlayerIngameLoginInfo _currentPlayerInfo;
    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;

    public Action InitDoneEvent;

    string _playerID;
    private Lobby _waitLobby;

    private bool isalready = false;
    public async Task<bool> InitLobbyScene()
    {
        try
        {
            // Unity Services �ʱ�ȭ
            await UnityServices.InitializeAsync();
            _playerID = await SignInAnonymouslyAsync();
            if (await JoinWaitLobby() is false)
            {
                await CreateWaitLobby();
            }
            await InputPlayerData();//�÷��̾��� ������ �ֱ�
            isalready = await IsAlreadyLogInID(_currentPlayerInfo.PlayerNickName);
            if (isalready is true)
            {
                return true;
            }

            
            InitDoneEvent?.Invoke();
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Initialization failed: {ex.Message}");
            return true;
        }
    }

    async Task<bool> JoinWaitLobby()
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions()//Call WaitLobby
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.Name, "WaitLobby", QueryFilter.OpOptions.EQ)
                }
            });


            if (queryResponse == null)
            {
                Debug.LogError("Failed to retrieve lobbies.");
                return false;
            }
            if (queryResponse.Results.Count > 0)
            {
                string lobbyId = queryResponse.Results[0].Id;
                _waitLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log("Successfully joined the waiting lobby.");
                return true;
            }
            else
            {
                Debug.Log("Waiting lobby not found.");
                return false;
            }
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            Debug.LogWarning("Rate limit exceeded. Retrying in 1 seconds...");
            await Task.Delay(1000); // 5�� ���
            return await JoinWaitLobby(); // ��õ�
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Lobby Service Exception: {e.Message}");
            return false;
        }
    }

    async Task CreateWaitLobby()
    {
        string lobbyName = "WaitLobby";
        int maxPlayers = 150;
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;

        _waitLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        if (_waitLobby != null)
            Debug.Log($"�κ� �������{_waitLobby}");
    }
    async Task<string> SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            string playerID = AuthenticationService.Instance.PlayerId;
            _currentPlayerInfo = new PlayerIngameLoginInfo(Managers.LogInManager.CurrentPlayerInfo.NickName, playerID);

            // Shows how to get the playerID
            return playerID;
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            return null;
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            return null;
        }
    }

    public async Task<bool> IsAlreadyLogInID(string usernickName)
    {
        try
        {
            // ��� �κ� �˻�
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in queryResponse.Results)
            {
               foreach(Player player in lobby.Players)
                {
                    foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                    {
                        if (_playerID == player.Id) //�ڱ��ڽ��� �ǳʶٱ�
                            continue;

                       if(data.Key != "NickName")
                        {
                            continue;
                        }
                        else
                        {
                            if(data.Value.Value != usernickName)
                            {
                                continue;
                            }
                            else
                            {
                                // �κ񿡼� �÷��̾� ����
                                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, _playerID);
                                // ���� ���� ���� ó��
                                SignOutPlayer();
                                return true;
                            }

                        }
                    }
                }
            }
            return false;
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            Debug.LogWarning("Rate limit exceeded. Retrying in 1 seconds...");
            await Task.Delay(1000); // 5�� ���
            return await IsAlreadyLogInID(usernickName); // ��õ�
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to query lobbies: {e.Message}");
            return false;
        }
    }
    private void SignOutPlayer()
    {
        try
        {
           AuthenticationService.Instance.SignOut();
            Debug.Log("Player signed out successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error signing out: {ex.Message}");
        }
    }
    private async Task InputPlayerData()
    {
        Player myPlayer = _waitLobby.Players.Find(player => player.Id == CurrentPlayerInfo.Id);

        if (myPlayer == null)
            return;

        Dictionary<string, PlayerDataObject> updatedData = new Dictionary<string, PlayerDataObject>
        {
            { "NickName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"{CurrentPlayerInfo.PlayerNickName}") },
        };

        // �ڽ��� �÷��̾� ������ ������Ʈ
        await LobbyService.Instance.UpdatePlayerAsync(_waitLobby.Id, myPlayer.Id, new UpdatePlayerOptions
        {
            Data = updatedData
        });
    }


    public async Task QuitApplication()
    {
        try
        {
            //Ensure you sign-in before calling Authentication Instance
            //See IAuthenticationService interface
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(_waitLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
}