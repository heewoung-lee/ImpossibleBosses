
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
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
    private PlayerIngameLoginInfo _currentPlayerInfo;
    private bool _isDoneInitEvent = false;
    private string _playerID;
    private Lobby _currentLobby;
    private bool isRefreshing = false;
    private bool isalready = false;
    private UI_Room_Inventory _ui_Room_Inventory;
    private ILobbyEvents _lobbyEvents;
    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;
    public Action InitDoneEvent;
    public Lobby CurrentLobby => _currentLobby;
    public bool IsDoneInitEvent { get => _isDoneInitEvent; }
    public UI_Room_Inventory UI_Room_Inventory
    {
        get
        {
            if (_ui_Room_Inventory == null)
            {
                _ui_Room_Inventory = Managers.UI_Manager.Get_Scene_UI<UI_Room_Inventory>();
            }
            return _ui_Room_Inventory;
        }
    }

    public async Task<bool> InitLobbyScene()
    {
        InitalizeVivoxEvent();
        try
        {
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Logging out from previous session...");
                AuthenticationService.Instance.SignOut();
            }
            await LogoutAndAllLeaveLobby();
            _playerID = await SignInAnonymouslyAsync();
            isalready = await IsAlreadyLogInID(_currentPlayerInfo.PlayerNickName);
            if (isalready is true)
            {
                return true;
            }
            await TryJoinLobbyByNameOrCreateWaitLobby();
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Initialization failed: {ex.Message}");
            return true;
        }
    }
    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        //하트비트 로비를 만든 호스트만 보낼 것 
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    public async Task TryJoinLobbyByNameOrCreateWaitLobby()
    {
        await TryJoinLobbyByNameOrCreateLobby("WaitLobby", 100, new CreateLobbyOptions()
        {
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
                {
                    {"WaitLobby",new DataObject(DataObject.VisibilityOptions.Public) }
                }
        });
    }

    public async Task TryJoinLobbyByNameOrCreateLobby(string lobbyName, int MaxPlayers, CreateLobbyOptions lobbyOption)
    {
        bool isLobbyJoinable = await TryJoinLobbyByName(lobbyName);
        if (isLobbyJoinable == false)
        {
            await CreateLobby(lobbyName, MaxPlayers, lobbyOption);
        }
    }



    async Task<bool> TryJoinLobbyByName(string lobbyName)
    {
        try
        {
            QueryResponse queryResponse = await GetQueryLobbiesAsyncCustom(new QueryLobbiesOptions()//Call WaitLobby
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.Name, lobbyName, QueryFilter.OpOptions.EQ)
                }
            });


            if (queryResponse == null)
            {
                Debug.LogError("Failed to retrieve lobbies.");
                return false;
            }
            if (queryResponse.Results.Count > 0)
            {
                foreach (var result in queryResponse.Results)
                {
                    Debug.Log(result.Id);
                }

                string lobbyId = queryResponse.Results[0].Id;
                Lobby lobby = await JoinLobbyByID(lobbyId);
                Debug.Log($"Successfully joined {lobbyName}lobby.");
                return true;
            }
            else
            {
                Debug.Log($"{lobbyName}Lobby not found.");
                return false;
            }
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await Utill.RateLimited(() => TryJoinLobbyByName(lobbyName)); // 재시도
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Lobby Service Exception: {e.Message}");
            return false;
        }
    }

    public async Task<Lobby> JoinLobbyByID(string lobbyID, string password = null)
    {
        Lobby currentlobby = _currentLobby;
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Password = password,
            };
            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, options);
        }
        catch (LobbyServiceException wrongPw) when (wrongPw.Reason == LobbyExceptionReason.IncorrectPassword)
        {
            Debug.Log("비밀번호가 틀렸습니다!");
            throw;
        }
        catch (LobbyServiceException timeLimit) when (timeLimit.Reason == LobbyExceptionReason.RateLimited)
        {
            await Utill.RateLimited(() => JoinLobbyByID(lobbyID, password), 2000);
        }
        catch (LobbyServiceException notfound) when (notfound.Reason == LobbyExceptionReason.LobbyNotFound)
        {
            throw;
        }
        catch (Exception error)
        {
            Debug.Log($"An Error Occured ErrorCode:{error}");
            return null;
        }
        await LeaveLobby(currentlobby);
        await JoinLobbyInitalize();
        return _currentLobby;
    }

    public async Task LeaveLobby(Lobby lobby)
    {
        if (_lobbyEvents != null)
        {
            await _lobbyEvents.UnsubscribeAsync();
            _lobbyEvents = null;
        }
        if (lobby != null)
        {
            await RemovePlayerData(lobby);
        }
    }
    public async Task CreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions options)
    {
        try
        {
            await LeaveLobby(_currentLobby);
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            if (_currentLobby != null)
                Debug.Log($"로비 만들어짐{_currentLobby.Name}");

            await JoinLobbyInitalize();
            Debug.Log($"로비 아이디: {_currentLobby.Id} \n 로비 이름{_currentLobby.Name}");
            Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(_currentLobby.Id, 15f));
        }

        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
    }

    private async Task JoinLobbyInitalize()
    {
        _isDoneInitEvent = false;
        try
        {
            await InputPlayerData(_currentLobby);
            await RegisterEvents(_currentLobby);
            await Managers.VivoxManager.JoinChannel(_currentLobby.Id);

            // 모든 작업이 완료될 때까지 대기
            InitDoneEvent?.Invoke();
            _isDoneInitEvent = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"JoinRoomInitalize 중 오류 발생: {ex.Message}");
            _isDoneInitEvent = false;
            throw; // 상위 호출부에 예외를 전달
        }

    }



    private async Task RegisterEvents(Lobby lobby)
    {
        LobbyEventCallbacks callBackEvent;
        callBackEvent = new LobbyEventCallbacks();
        callBackEvent.PlayerJoined += PlayerJoinedEvent;
        callBackEvent.PlayerLeft += async (List<int> list) =>
        {
            await PlayerLeftEvent(list);
        };
        try
        {
            Debug.Log($"현재 로비{_currentLobby.Name} 파라미터로비 {lobby.Name} ");
            _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callBackEvent);
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
    }
    private async Task PlayerLeftEvent(List<int> list)
    {
        try
        {
            Debug.Log("업데이트");
            await ReFreshRoomList();
            await GetLobbyAsyncCustom(_currentLobby.Id);
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            await Utill.RateLimited(() => PlayerLeftEvent(list), 2000);
        }
    }


    private async Task<Lobby> GetLobbyAsyncCustom(string lobbyID)
    {
        try
        {
            return await LobbyService.Instance.GetLobbyAsync(lobbyID);
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await Utill.RateLimited(() => GetLobbyAsyncCustom(lobbyID));
        }
    }
    private async Task<QueryResponse> GetQueryLobbiesAsyncCustom(QueryLobbiesOptions queryFilter = null)
    {
        try
        {
            return await LobbyService.Instance.QueryLobbiesAsync(queryFilter);
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await Utill.RateLimited(() => GetQueryLobbiesAsyncCustom(queryFilter));
        }
    }

    private async void PlayerJoinedEvent(List<LobbyPlayerJoined> joined)
    {
        try
        {
            await Managers.VivoxManager.SendSystemMessageAsync($"{CurrentPlayerInfo.PlayerNickName}님이 입장하셨습니다.");
            Debug.Log($"{CurrentPlayerInfo.PlayerNickName}님이 입장하셨습니다.");
        }
        catch (Exception ex)
        {
            Debug.Log($"에러가 발생했습니다 : 로비ID{_currentLobby.Id} 에러코드: {ex}");
        }
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
    async Task<string> SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            string playerID = AuthenticationService.Instance.PlayerId;
            Debug.Log($"플레이어 ID 만들어짐{playerID}");
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
            QueryResponse queryResponse = await GetQueryLobbiesAsyncCustom();

            foreach (Lobby lobby in queryResponse.Results)
            {
                foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
                {

                    if (player.Data == null)
                    {
                        Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                        return await Utill.RateLimited(() => IsAlreadyLogInID(usernickName)); // 재시도
                    }
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
            return await Utill.RateLimited(() => IsAlreadyLogInID(usernickName)); // 재시도
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to query lobbies: {ex.Message}");
            return false;
        }
    }



    private async Task InputPlayerData(Lobby lobby)
    {

        Dictionary<string, PlayerDataObject> updatedData = new Dictionary<string, PlayerDataObject>
        {
            { "NickName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"{_currentPlayerInfo.PlayerNickName}") },
        };
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, _currentPlayerInfo.Id, new UpdatePlayerOptions
            {
                Data = updatedData
            });

            Debug.Log($"로비ID{lobby.Id} \t 플레이어ID{_currentPlayerInfo.Id} 정보가 입력되었습니다.");
        }
        catch (ArgumentException alreadyAddKey) when (alreadyAddKey.Message.Contains("An item with the same key has already been added"))
        {
            Debug.Log($"{alreadyAddKey}이미 키가 있음 무시해도 됨");
        }

    }


    private async Task RemovePlayerData(Lobby lobby)
    {
        Debug.Log($"로비ID{lobby.Id} \t 플레이어ID{_currentPlayerInfo.Id} 정보가 제거되었습니다.");
        await LobbyService.Instance.RemovePlayerAsync(lobby.Id, _currentPlayerInfo.Id);
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
        catch (LobbyServiceException ex) when (ex.Reason == LobbyExceptionReason.RateLimited)
        {
            Debug.Log($"Failed to remove player from lobby: {ex.Message} 다시 시도중");
            await Utill.RateLimited(() => LogoutAndAllLeaveLobby());
        }

        // 사용자 인증 로그아웃
        AuthenticationService.Instance.SignOut();
        AuthenticationService.Instance.ClearSessionToken();
        _currentPlayerInfo = default;
        _currentLobby = null;
        Debug.Log("User signed out successfully.");
    }

    public async Task<List<string>> ViewCurrentPlayerLobby()
    {
        return await LobbyService.Instance.GetJoinedLobbiesAsync();
    }

    public async Task<List<Lobby>> ViewShowAllLobby()
    {
        try
        {
            QueryResponse lobbies = await GetQueryLobbiesAsyncCustom();
            return lobbies.Results; // 로비 목록 반환
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await Utill.RateLimited(() => ViewShowAllLobby()); // 재시도
        }
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutAndAllLeaveLobby;
    }
    private async Task ShowUpdatedLobbyPlayers()
    {

        QueryLobbiesOptions viewOption = new QueryLobbiesOptions();
        viewOption.Count = 25;
        viewOption.Filters = new List<QueryFilter>()
            {
            new QueryFilter(
            field: QueryFilter.FieldOptions.AvailableSlots,
            op: QueryFilter.OpOptions.GT,
            value: "0"),
            };
        try
        {
            QueryResponse lobbies = await GetQueryLobbiesAsyncCustom(viewOption);
            foreach (var lobby in lobbies.Results)
            {
                Debug.Log($"현재 로비이름 {lobby.Name}");
                Debug.Log($"-----------------------------------");
                foreach (var player in lobby.Players)
                {
                    Debug.Log($"플레이어 아이디: {player.Id}");
                }
                Debug.Log($"-----------------------------------");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to fetch updated lobby info: {e.Message}");
        }
        catch (Exception ex)
        {
            Debug.Log($"에러{ex}");
        }
    }


    public async Task ReFreshRoomList()
    {
        if (isRefreshing) { return; }

        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;
            options.Filters = new List<QueryFilter>()
            {
            new QueryFilter(
            field: QueryFilter.FieldOptions.S1,
            op: QueryFilter.OpOptions.EQ,
            value: "CharactorSelect"),
            new QueryFilter(
            field: QueryFilter.FieldOptions.AvailableSlots,
            op: QueryFilter.OpOptions.GT,
            value: "0"),
            };
            QueryResponse lobbies = await GetQueryLobbiesAsyncCustom(options);

            foreach (Transform child in UI_Room_Inventory.Room_Content)
            {
                Managers.ResourceManager.DestroyObject(child.gameObject);
            }
            foreach (Lobby lobby in lobbies.Results)
            {
                CreateRoomInitalize(lobby);
            }
            await ShowUpdatedLobbyPlayers();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }

    public void CreateRoomInitalize(Lobby lobby)
    {
        UI_Room_Info_Panel infoPanel = Managers.UI_Manager.MakeSubItem<UI_Room_Info_Panel>(UI_Room_Inventory.Room_Content);
        infoPanel.SetRoomInfo(lobby);

        Debug.Log($"방에 들어간 로비 ID{lobby.Id}");
    }
}