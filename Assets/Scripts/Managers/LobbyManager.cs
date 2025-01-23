
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
    private LobbyEventCallbacks _callBackEvent;
    private ILobbyEvents _lobbyEvents;

    private bool isalready = false;

    public async Task<bool> InitLobbyScene()
    {
        InitalizeVivoxEvent();
        try
        {
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            _playerID = await SignInAnonymouslyAsync();
            Lobby lobby = null;



            lobby = await TryJoinLobbyByNameOrCreateLobby("WaitLobby",100, new CreateLobbyOptions()
            {
                IsPrivate = false
            });



            await InputPlayerData(lobby);//플레이어의 데이터 넣기
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


    public async Task<Lobby> TryJoinLobbyByNameOrCreateLobby(string lobbyName,int MaxPlayers, CreateLobbyOptions lobbyOption)
    {
        Lobby lobby = null;
        lobby = await TryJoinLobbyByName(lobbyName);
        if (lobby == null)
        {
            lobby = await CreateLobby(lobbyName, MaxPlayers, lobbyOption);
        }
        return lobby;
    }


    async Task<Lobby> TryJoinLobbyByName(string lobbyName)
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions()//Call WaitLobby
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.Name, lobbyName, QueryFilter.OpOptions.EQ)
                }
            });


            if (queryResponse == null)
            {
                Debug.LogError("Failed to retrieve lobbies.");
                return null;
            }
            if (queryResponse.Results.Count > 0)
            {
                foreach(var result in queryResponse.Results)
                {
                    Debug.Log(result.Id);
                }

                string lobbyId = queryResponse.Results[0].Id;
                Lobby lobby = await TryJoinLobbyByID(lobbyId);

                Debug.Log("Successfully joined the waiting lobby.");
                return lobby;
            }
            else
            {
                Debug.Log("Waiting lobby not found.");
                return null;
            }
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await RateLimited(() => TryJoinLobbyByName(lobbyName)); // 재시도
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Lobby Service Exception: {e.Message}");
            return null;
        }
    }

    public async Task<Lobby> TryJoinLobbyByID(string lobbyID)
    {
        await LeaveAllLobby();
        Lobby lobby = null;
        try
        {
            lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
            await RegisterEvents(lobby);
            if (lobby == null)
                return null;
        }
        catch(Exception error)
        {
            Debug.Log($"An Error Occured ErrorCode:{error}");
            return null;
        }
        return lobby;
    }
    private async Task LeaveAllLobby()
    {
        string playerId = AuthenticationService.Instance.PlayerId;
        List<string> lobbyes = await LobbyService.Instance.GetJoinedLobbiesAsync();
        foreach (string lobby in lobbyes)
        {
            await LobbyService.Instance.RemovePlayerAsync(lobby, playerId); //먼저 로그인되어있는 로비에서 전부 나가기
            Debug.Log($"{lobby}에서 나갔습니다.");
        }//먼저 연결되어있는 로비 전부제거.
    }


    async Task<Lobby> CreateLobby(string lobbyName,int maxPlayers, CreateLobbyOptions options)
    {
        try
        {
            await LeaveAllLobbyes();
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            if (lobby != null)
                Debug.Log($"로비 만들어짐{lobby}");

            await RegisterEvents(lobby);
            Debug.Log($"로비 아이디: {lobby.Id} \n 로비 이름{lobby.Name}");

            return lobby;
        }

        catch(Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
    
    }

    private async Task RegisterEvents(Lobby lobby)
    {
        _callBackEvent = new LobbyEventCallbacks();
        _callBackEvent.PlayerJoined += PlayerJoined;
        try
        {
            _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, _callBackEvent);
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{lobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                default: throw;
            }
        }
        //이벤트 구독

    }


    private async void PlayerJoined(List<LobbyPlayerJoined> joined)
    {
        await Managers.VivoxManager.SendSystemMessageAsync($"{CurrentPlayerInfo.PlayerNickName}님이 입장하셨습니다.");
        Debug.Log($"{CurrentPlayerInfo.PlayerNickName}님이 입장하셨습니다.");
    }
    private async Task LeaveAllLobbyes()
    {
        string playerId = AuthenticationService.Instance.PlayerId;
        List<string> lobbyes = await LobbyService.Instance.GetJoinedLobbiesAsync();
        foreach (string lobbyID in lobbyes)
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyID, playerId); //먼저 로그인되어있는 로비에서 전부 나가기
            Debug.Log($"{lobbyID}에서 나갔습니다.");
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
            return lobby.Id;
        }
        catch
        {
            throw;
        }
    }
    public async Task LeaveLobby()
    {
        Lobby currentLobby = await GetCurrentLobby();
        await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, _playerID);
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
                                await LogoutAndAllLeaveLobby();
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
    private async Task InputPlayerData(Lobby lobby)
    {
        Player myPlayer = lobby.Players.Find(player => player.Id == CurrentPlayerInfo.Id);

        if (myPlayer == null)
            return;

        Dictionary<string, PlayerDataObject> updatedData = new Dictionary<string, PlayerDataObject>
        {
            { "NickName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"{CurrentPlayerInfo.PlayerNickName}") },
        };

        // 자신의 플레이어 데이터 업데이트
        await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, myPlayer.Id, new UpdatePlayerOptions
        {
            Data = updatedData
        });
    }

    public async Task LogoutAndAllLeaveLobby()
    {
        if (AuthenticationService.Instance.IsSignedIn == false)
            return;

        try
        {
            await LeaveAllLobby();
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

    public async Task<Lobby> GetCurrentLobby()
    {
        try
        {
            List<string> lobbyes = await LobbyService.Instance.GetJoinedLobbiesAsync();
            if (lobbyes == null)
            {
                Debug.Log("Player hasn't Joined Lobby");
                return null;
            }
            Debug.Log($"만들어진 로비ID:{lobbyes[0]}");

            foreach( string lobby in lobbyes)
            {
                Debug.Log($"로비목록: {lobby}");
            }
            return await LobbyService.Instance.GetLobbyAsync(lobbyes[0]);

        }
        catch(Exception ex)
        {
            Debug.Log($"An Error Occuent{ex}");
            return null;    
        }
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutAndAllLeaveLobby;
    }
}