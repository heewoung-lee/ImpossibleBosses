
using System;
using System.Collections.Generic;
using System.Data;
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
        _nickname = playerNickname; // 모든 필드를 초기화
        _Id = playerId;
    }
}


public class LobbyManager : IManagerEventInitailize
{
    PlayerIngameLoginInfo _currentPlayerInfo;
    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;

    public Action InitDoneEvent;

    string _playerID;
    private Lobby _currentLobby;
    public Lobby CurrentLobby => _currentLobby;

    private bool isalready = false;

    public async Task<bool> InitLobbyScene()
    {
        InitalizeVivoxEvent();

        try
        {
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            _playerID = await SignInAnonymouslyAsync();
            if (await TryJoinWaitLobby() is false)
            {
                await CreateWaitLobby();
            }
            await InputPlayerData();//플레이어의 데이터 넣기
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

    async Task<bool> TryJoinWaitLobby()
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
                _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
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
            return await RateLimited(() => TryJoinWaitLobby()); // 재시도
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
        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        if (_currentLobby != null)
            Debug.Log($"로비 만들어짐{_currentLobby}");
    }

    async Task TryJoinOrCreateWaitLobby()
    {
        if (await TryJoinWaitLobby() == false)
        {
            await CreateWaitLobby();
        }
    }

    public async Task<string> CreateRoom(string roomName, int maxPlayers, int? roomPW = default)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            if (roomPW.HasValue)
            {
                options.Password = roomPW.Value.ToString();
            }
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(roomName, maxPlayers, options);
            _currentLobby = lobby;
            return lobby.Id;
        }
        catch
        {
            throw;
        }
    }
    public async Task LeaveLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, _playerID);
        _currentLobby = null;
    }

    public async Task JoinRoom(string roomId,int? roomPw = default)
    {
        try
        {
            JoinLobbyByIdOptions idOptions = null;
            if (roomPw.HasValue)
            {
                idOptions = new JoinLobbyByIdOptions { Password = roomPw.Value.ToString() };
            }
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(roomId, idOptions);
            _currentLobby = lobby;
        }
        catch
        {
            throw;
        }
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
            // 모든 로비 검색
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in queryResponse.Results)
            {
                foreach (Player player in lobby.Players)
                {
                    foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                    {
                        if (_playerID == player.Id) //자기자신은 건너뛰기
                            continue;

                        if (data.Key != "NickName")
                        {
                            continue;
                        }
                        else
                        {
                            if (data.Value.Value != usernickName)
                            {
                                continue;
                            }
                            else
                            {
                                await LogoutAndLeaveLobby();
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
            return await RateLimited(() => IsAlreadyLogInID(usernickName)); // 재시도
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to query lobbies: {e.Message}");
            return false;
        }
    }

    private async Task<T> RateLimited<T>(Func<Task<T>> action, int millisecondsDelay = 1000)
    {
        Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
        await Task.Delay(millisecondsDelay); // 대기
        return await action.Invoke(); // 전달받은 작업 실행 및 결과 반환
    }
    private async Task InputPlayerData()
    {
        Player myPlayer = _currentLobby.Players.Find(player => player.Id == CurrentPlayerInfo.Id);

        if (myPlayer == null)
            return;

        Dictionary<string, PlayerDataObject> updatedData = new Dictionary<string, PlayerDataObject>
        {
            { "NickName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"{CurrentPlayerInfo.PlayerNickName}") },
        };

        // 자신의 플레이어 데이터 업데이트
        await LobbyService.Instance.UpdatePlayerAsync(_currentLobby.Id, myPlayer.Id, new UpdatePlayerOptions
        {
            Data = updatedData
        });
    }

    public async Task LogoutAndLeaveLobby()
    {
        if (AuthenticationService.Instance.IsSignedIn == false)
            return;

        try
        {
            // 로비에서 사용자 제거
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, _playerID);
            Debug.Log("Player removed from lobby.");
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"Failed to remove player from lobby: {ex.Message}");
        }

        // 사용자 인증 로그아웃
        AuthenticationService.Instance.SignOut();
        _currentPlayerInfo = default;
        Debug.Log("User signed out successfully.");
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutAndLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutAndLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutAndLeaveLobby;
    }
}