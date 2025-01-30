
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
    private LobbyEventCallbacks _callBackEvent;
    private bool isRefreshing = false;
    private ILobbyEvents _lobbyEvents;
    private bool isalready = false;
    private UI_Room_Inventory _ui_Room_Inventory;
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
            _playerID = await SignInAnonymouslyAsync();
            isalready = await IsAlreadyLogInID(_currentPlayerInfo.PlayerNickName);
            if (isalready is true)
            {
                return true;
            }
            await TryJoinLobbyByNameOrCreateLobby("WaitLobby", 100, new CreateLobbyOptions()
            {
                IsPrivate = false
            });


            Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(_currentLobby.Id, 15f));
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
            return await RateLimited(() => TryJoinLobbyByName(lobbyName)); // 재시도
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Lobby Service Exception: {e.Message}");
            return false;
        }
    }

    public async Task<Lobby> JoinLobbyByID(string lobbyID)
    {
        await LeaveCurrentLobby();
        try
        {
            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
            await JoinRoomInitalize();
        }
        catch (Exception error)
        {
            Debug.Log($"An Error Occured ErrorCode:{error}");
            return null;
        }
        return _currentLobby;
    }


    public async Task LeaveCurrentLobby()
    {
        if (_lobbyEvents != null)
        {
            await _lobbyEvents.UnsubscribeAsync();
            _lobbyEvents = null;
        }
        if (_currentLobby != null)
        {
            Debug.Log("로비에서 플레이어 데이터 삭제");
            await RemovePlayerData();
        }
    }
    public async Task CreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions options)
    {
        try
        {
            await LeaveCurrentLobby();
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            if (_currentLobby != null)
                Debug.Log($"로비 만들어짐{_currentLobby.Name}");

            await JoinRoomInitalize();
            Debug.Log($"로비 아이디: {_currentLobby.Id} \n 로비 이름{_currentLobby.Name}");
        }

        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
    }

    private async Task JoinRoomInitalize()
    {
        _isDoneInitEvent = false;
        try
        {
            //Task registerEventsTask = RegisterEvents(_currentLobby);
            //Task inputPlayerDataTask = InputPlayerData(_currentLobby);
            //Task joinChaanel = Managers.VivoxManager.JoinChannel(_currentLobby.Id);
            //await Task.WhenAll(registerEventsTask, inputPlayerDataTask, joinChaanel);


            await RegisterEvents(_currentLobby);
            //await InputPlayerData(_currentLobby);
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
        _callBackEvent = new LobbyEventCallbacks();
        //_callBackEvent.PlayerJoined += PlayerJoinedEvent;
        //_callBackEvent.PlayerLeft += async (List<int> list) =>
        //{
        //    await PlayerLeftEvent(list);
        //};
        //try
        //{
        //    _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, _callBackEvent);
        //}
        //catch (LobbyServiceException ex)
        //{
        //    switch (ex.Reason)
        //    {
        //        case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{lobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
        //        case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
        //        case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
        //        default: throw;
        //    }
        //}
        //이벤트 구독

    }
    private async Task PlayerLeftEvent(List<int> list)
    {
        try
        {

            Debug.Log("업데이트");
            await ReFreshRoomList();
            await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            await RateLimited(() => PlayerLeftEvent(list));
        }
    }

    private async void PlayerJoinedEvent(List<LobbyPlayerJoined> joined)
    {
        //TODO:들어온 Player목록으로 바꿔야함
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
                foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
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
    private async Task RateLimited(Func<Task> action, int millisecondsDelay = 1000)
    {
        Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
        await Task.Delay(millisecondsDelay); // 대기
        await action.Invoke(); // 전달받은 작업 실행 및 결과 반환
    }


    private async Task InputPlayerData(Lobby lobby)
    {
        Unity.Services.Lobbies.Models.Player myPlayer = lobby.Players.Find(player => player.Id == CurrentPlayerInfo.Id);

        if (myPlayer == null)
            return;

        Dictionary<string, PlayerDataObject> updatedData = new Dictionary<string, PlayerDataObject>
        {
            { "NickName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"{CurrentPlayerInfo.PlayerNickName}") },
        };
        try
        {
            // 자신의 플레이어 데이터 업데이트
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, myPlayer.Id, new UpdatePlayerOptions
            {

                Data = updatedData
            });
            Debug.Log($"로비에 플레이어정보넣기{lobby.Id}{myPlayer.Id}");

        }
        catch (ArgumentException alreadyAddKey) when (alreadyAddKey.Message.Contains("An item with the same key has already been added"))
        {
            Debug.Log($"{alreadyAddKey}이미 키가 있음 무시해도 됨");
        }

    }


    private async Task RemovePlayerData()
    {
        if (_currentLobby == null)
            return;

        Unity.Services.Lobbies.Models.Player myPlayer = _currentLobby.Players.Find(player => player.Id == CurrentPlayerInfo.Id);
        if (myPlayer == null)
            return;


        Debug.Log($"{_currentLobby.Id}{myPlayer.Id}가 발견됨");
        await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, myPlayer.Id);
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
            await RateLimited(() => LogoutAndAllLeaveLobby());
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
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync();
            return lobbies.Results; // 로비 목록 반환
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await RateLimited(() => ViewShowAllLobby()); // 재시도
        }
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutAndAllLeaveLobby;
    }
    private async Task ShowUpdatedLobbyPlayers(Lobby currentLobby)
    {
        try
        {
            // 서버에서 현재 로비의 최신 상태를 다시 받아온다
            Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            Debug.Log($"Lobby ID: {updatedLobby.Id}, Name: {updatedLobby.Name}");

            // 최신 로비에 있는 플레이어 목록 순회
            foreach (var player in updatedLobby.Players)
            {
                Debug.Log($"Player ID: {player.Id}");

                // 예: Player Data에 "NickName" 키가 있는지 확인
                if (player.Data.ContainsKey("NickName"))
                {
                    Debug.Log($"NickName: {player.Data["NickName"].Value}");
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to fetch updated lobby info: {e.Message}");
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
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in UI_Room_Inventory.Room_Content)
            {
                Managers.ResourceManager.DestroyObject(child.gameObject);
            }
            foreach (Lobby lobby in lobbies.Results)
            {
                UI_Room_Info_Panel infoPanel = Managers.UI_Manager.MakeSubItem<UI_Room_Info_Panel>(UI_Room_Inventory.Room_Content);

                //Managers.ResourceManager.Instantiate(); TODO: 여기에 UI 생성
                //여기에 룸정보 입력하기.방제 인원 등
            }
            Debug.Log($"호스트ID{_currentLobby.HostId}");
            await ShowUpdatedLobbyPlayers(_currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }
}