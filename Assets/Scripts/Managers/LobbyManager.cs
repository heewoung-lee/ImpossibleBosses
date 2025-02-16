using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using Player = Unity.Services.Lobbies.Models.Player;



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

public class LobbyManager : IManagerEventInitailize, ILoadingSceneTaskChecker
{
    enum LoadingProcess
    {
        VivoxInitalize,
        UnityServices,
        SignInAnonymously,
        CheckAlreadyLogInID,
        TryJoinLobby,
        VivoxLogin
    }
    private const string LOBBYID = "WaitLobbyRoom85";
    private PlayerIngameLoginInfo _currentPlayerInfo;
    private bool _isDoneInitEvent = false;
    private string _playerID;
    private Lobby _currentLobby;
    private bool isRefreshing = false;
    private bool isalready = false;
    private UI_Room_Inventory _ui_Room_Inventory;
    private Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> _playersJoinLobbyDict;
    private bool[] _taskChecker;
    private Coroutine _heartBeatCoroutine = null;


    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;
    public Action InitDoneEvent;
    public Action<string> PlayerAddDataInputEvent;
    public Action<int> PlayerDeleteEvent;
    public Action<bool> LobbyLoading;
    public Func<string,Task<bool>> HostChangedEvent;


    public Lobby CurrentLobby => _currentLobby;
    public bool IsDoneInitEvent { get => _isDoneInitEvent; }
    public bool[] TaskChecker => _taskChecker;

    public async Task<bool> InitLobbyScene()
    {
        _taskChecker = new bool[Enum.GetValues(typeof(LoadingProcess)).Length];
        LoadingScene.SetCheckTaskChecker(_taskChecker);
        Managers.VivoxManager.VivoxDoneLoginEvent -= SetVivoxTaskCheker;
        Managers.VivoxManager.VivoxDoneLoginEvent += SetVivoxTaskCheker;
        InitalizeVivoxEvent();
        Managers.RelayManager.DisconnectPlayerEvent -= DisconnectPlayer;
        Managers.RelayManager.DisconnectPlayerEvent += DisconnectPlayer;
        _taskChecker[(int)LoadingProcess.VivoxInitalize] = true;
        try
        {
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            _taskChecker[(int)LoadingProcess.UnityServices] = true;
            if (AuthenticationService.Instance.IsSignedIn)
            {
                await LogoutAndAllLeaveLobby();
                Debug.Log("Logging out from previous session...");
                AuthenticationService.Instance.SignOut();

            }
            _playerID = await SignInAnonymouslyAsync();
            _taskChecker[(int)LoadingProcess.SignInAnonymously] = true;
            isalready = await IsAlreadyLogInID(_currentPlayerInfo.PlayerNickName);
            if (isalready is true)
            {
                Debug.Log("이미 접속되어있음");
                return true;
            }
            _taskChecker[(int)LoadingProcess.CheckAlreadyLogInID] = true;
            await TryJoinLobbyByNameOrCreateWaitLobby();
            _taskChecker[(int)LoadingProcess.TryJoinLobby] = true;
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Initialization failed: {ex.Message}");
            if (Managers.SceneManagerEx.GetCurrentScene is LoadingScene loadingScene)
            {
                loadingScene.IsErrorOccurred = true;
            }
            throw;
        }
    }

    private void SetVivoxTaskCheker()
    {
        _taskChecker[(int)LoadingProcess.VivoxLogin] = true;
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("HeartBeat is Running");
            yield return delay;
        }
    }

    public async Task CheckHostAndSendsHeartBeat(Lobby lobby, float interval = 15f)
    {
        if (lobby == null || _playerID == null)
            return;

        StopHeartbeat();
        if (_currentLobby.HostId == _playerID)
        {
            Debug.Log("하트비트 이식");
            _heartBeatCoroutine = Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, interval));
            string joincode = await Managers.RelayManager.StartHostWithRelay(lobby.MaxPlayers);
            await InjectionRelayJoinCodeintoLobby(_currentLobby, joincode);
        }
        else
        {
            try
            {
                string joincode = lobby.Data["RelayCode"].Value;
                await Managers.RelayManager.JoinGuestRelay(joincode);
            }
            catch (KeyNotFoundException exception)
            {
                Debug.Log($"릴레이 코드가 없으므로 다시 실행합니다.{exception}");
                await Utill.RateLimited(() => CheckHostAndSendsHeartBeat(lobby));
            }
        }
    }

    private void StopHeartbeat()
    {
        if (_heartBeatCoroutine != null)
        {
            Managers.ManagersStopCoroutine(_heartBeatCoroutine);
            _heartBeatCoroutine = null;
        }
    }

    public async Task TryJoinLobbyByNameOrCreateWaitLobby()
    {
        if (_currentLobby != null)
            await LeaveLobby(_currentLobby); //이쪽은 문제 없음

        try
        {
            await TryJoinLobbyByNameOrCreateLobby("WaitLobby", 100, new CreateLobbyOptions()
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {"WaitLobby",new DataObject(DataObject.VisibilityOptions.Public,"PlayerWaitLobbyRoom") }
                }
            });
        }

        catch (LobbyServiceException playerNotFound) when (playerNotFound.Message.Contains("player not found"))
        {
            Debug.Log("Player Not Found");
            await InitLobbyScene();
            return;
        }
        catch (Exception e)
        {
            Debug.Log("여기서 문제가 발생" + e);
        }
    }

    public async Task TryJoinLobbyByNameOrCreateLobby(string lobbyName, int MaxPlayers, CreateLobbyOptions lobbyOption)
    {
        try
        {
            _currentLobby = await LobbyService.Instance.CreateOrJoinLobbyAsync(LOBBYID, lobbyName, MaxPlayers, lobbyOption);
            await CheckHostAndSendsHeartBeat(_currentLobby);
            await JoinLobbyInitalize(_currentLobby);
        }
        catch (LobbyServiceException alreayException) when (alreayException.Message.Contains("player is already a member of the lobby"))
        {
            Debug.Log("플레이어가 이미 접속중입니다. 정보삭제후 재진입을 시도 합니다.");
            await InitLobbyScene();
        }
        catch (LobbyServiceException TimeLimmitException) when (TimeLimmitException.Message.Contains("Rate limit has been exceeded"))
        {
            await Utill.RateLimited(() => TryJoinLobbyByNameOrCreateLobby(lobbyName, MaxPlayers, lobbyOption));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
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
            return await Utill.RateLimited(() => JoinLobbyByID(lobbyID, password), 2000);
        }
        catch (LobbyServiceException notfound) when (notfound.Reason == LobbyExceptionReason.LobbyNotFound)
        {
            Debug.Log($"LobbyNotFound{notfound.Message}");
            throw;
        }
        catch (Exception error)
        {
            Debug.Log($"An Error Occured ErrorCode:{error}");
            return null;
        }
        await LeaveLobby(currentlobby);
        await JoinLobbyInitalize(_currentLobby);
        return _currentLobby;
    }

    public async Task LeaveLobby(Lobby lobby)
    {
        try
        {
            if (lobby != null)
            {
                await RemovePlayerData(lobby);
            }
        }
        catch (System.ObjectDisposedException disposedException)
        {
            Debug.Log($"이미 객체가 제거되었습니다.{disposedException.Message}");
        }
        catch (Exception e)
        {
            Debug.Log($"LeaveLobby 메서드 안에서의 에러{e}");
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
            await JoinLobbyInitalize(_currentLobby);
            await CheckHostAndSendsHeartBeat(_currentLobby);
            Debug.Log("조인코드 삽입");
        }

        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
    }
    private async Task InjectionRelayJoinCodeintoLobby(Lobby lobby, string joincode)
    {
        if (joincode == null || lobby == null)
        {
            Debug.Log($"Data has been NULL, is Check Lobby Null?: {lobby.Equals(null)} is Check JoinCode Null? {lobby.Equals(null)}");
            return;
        }

        await UpdateLobbyRelayCode(lobby, joincode);
    }

    private async Task JoinLobbyInitalize(Lobby lobby)
    {
        _isDoneInitEvent = false;
        try
        {
            await InputPlayerData(lobby);
            await Managers.VivoxManager.JoinChannel(lobby.Id);
            CallbackInitalize(lobby);
            InitDoneEvent?.Invoke();
            _isDoneInitEvent = true;
        }

        catch (Exception ex)
        {
            Debug.LogError($"JoinRoomInitalize 중 오류 발생: {ex}");
            _isDoneInitEvent = false;
            throw; // 상위 호출부에 예외를 전달
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



    private async Task LeaveAllLobby()
    {
        LobbyLoading?.Invoke(true);
        List<Lobby> lobbyinPlayerList = await CheckAllLobbyinPlayer();
        foreach (Lobby lobby in lobbyinPlayerList)
        {
            await LeaveLobby(lobby);
            Debug.Log($"{lobby}에서 나갔습니다.");
            StopHeartbeat();
        }
        LobbyLoading?.Invoke(false);
    }


    private async Task<List<Lobby>> CheckAllLobbyinPlayer()
    {
        List<Lobby> lobbyinPlayerList = new List<Lobby>();
        QueryResponse allLobbyResponse = await GetQueryLobbiesAsyncCustom();


        foreach (Lobby lobby in allLobbyResponse.Results)
        {
            if (lobby.Players.Any(player => player.Id == _playerID))
            {
                lobbyinPlayerList.Add(lobby);
            }
        }
        return lobbyinPlayerList;
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

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();

            if (response.Results.Count <= 0)
                return false;

            foreach (Lobby lobby in response.Results)
            {
                foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
                {
                    if (player.Data == null && _playerID == player.Id)//나인데 데이터를 할당 못받았으면,다시 초기화
                    {
                        Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                        return await Utill.RateLimited(() => InitLobbyScene(), 5000); // 재시도
                    }
                    foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                    {
                        if (player.Data == null)
                            continue;

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

        catch (Exception notSetObjectException) when (notSetObjectException.Message.Equals("Object reference not set to an instance of an object"))
        {
            Debug.Log("The Lobby hasnt reference so We Rate Secend");
          return  await Utill.RateLimited(() => IsAlreadyLogInID(usernickName));
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

            Debug.Log($"로비ID: {lobby.Id} \t 플레이어ID: {_currentPlayerInfo.Id} 정보가 입력되었습니다.");
        }
        catch (Exception error)
        {
            Debug.LogError($"에러 발생{error}");
        }
    }


    private async Task RemovePlayerData(Lobby lobby)
    {
        Debug.Log($"로비ID{lobby.Id} \t 플레이어ID{_playerID} 정보가 제거되었습니다.");
        await LobbyService.Instance.RemovePlayerAsync(lobby.Id, _playerID);
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
        catch (Exception ex)
        {
            Debug.Log($"에러발생: {ex}");
        }
        // 사용자 인증 로그아웃
        AuthenticationService.Instance.SignOut();
        AuthenticationService.Instance.ClearSessionToken();
        _currentPlayerInfo = default;
        _currentLobby = null;
        Debug.Log("User signed out successfully.");
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutAndAllLeaveLobby;
    }
    


    public async Task ReFreshRoomList()
    {
        if (_currentLobby == null)
            return;


        if (isRefreshing || Managers.UI_Manager.Try_Get_Scene_UI(out UI_Room_Inventory room_inventory_ui) == false)
        {
            return;
        }

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

            foreach (Transform child in room_inventory_ui.Room_Content)
            {
                Managers.ResourceManager.DestroyObject(child.gameObject);
            }
            foreach (Lobby lobby in lobbies.Results)
            {
                CreateRoomInitalize(lobby);
            }
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

        if (Managers.UI_Manager.Try_Get_Scene_UI(out UI_Room_Inventory room_inventory_ui) == false)
            return;

        UI_Room_Info_Panel infoPanel = Managers.UI_Manager.MakeSubItem<UI_Room_Info_Panel>(room_inventory_ui.Room_Content);
        infoPanel.SetRoomInfo(lobby);
    }

    public void CallbackInitalize(Lobby lobby)
    {
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.LobbyChanged += async (lobbyChange) =>
        {
            if (lobbyChange.HostId.Value == _playerID)
            {
                await OnChangeHostPlayer();
            }
        };
        LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }
    public async Task OnChangeHostPlayer()
    {
        string joinCode = await Managers.RelayManager.StartHostWithRelay(_currentLobby.MaxPlayers);

        await UpdateLobbyRelayCode(_currentLobby, joinCode);
    }


    private async Task UpdateLobbyRelayCode(Lobby lobbby,string joinCode)
    {
        await LobbyService.Instance.UpdateLobbyAsync(lobbby.Id, new UpdateLobbyOptions()
        {
            Data = new Dictionary<string, DataObject>
                {
                   {"RelayCode",new DataObject(DataObject.VisibilityOptions.Public,joinCode)}
                }
        });
    }



    public async Task DisconnectPlayer()
    {
        LobbyLoading?.Invoke(true);
        Debug.Log($"현재 로비의 호스트 ID{_currentLobby.HostId}");
        LobbyLoading?.Invoke(false);
    }



    #region TestDebugCode
    public void ShowLobbyData()
    {
        foreach(var data in _currentLobby.Data)
        {
            Debug.Log($"{data.Key}의 값은 {data.Value.Value}");
        }
    }
    public async Task ShowUpdatedLobbyPlayers()
    {
        try
        {
            QueryResponse lobbies = await GetQueryLobbiesAsyncCustom();
            foreach (var lobby in lobbies.Results)
            {
                Player hostPlayer = lobby.Players.FirstOrDefault(player => player.Id == lobby.HostId);

                Debug.Log($"현재 로비이름: {lobby.Name} 로비ID: {lobby.Id} 호스트닉네임: {hostPlayer.Data["NickName"].Value} 로비호스트: {lobby.HostId} ");
                Debug.Log($"-----------------------------------");
                foreach (var player in lobby.Players)
                {
                    Debug.Log($"플레이어 아이디: {player.Id} 플레이어 닉네임:{player.Data["NickName"].Value}");
                }
                Debug.Log($"-----------------------------------");
            }
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RequestTimeOut)
        {
            Debug.LogError($"RequestTimeOut");
            await Utill.RateLimited(async () => { await ShowUpdatedLobbyPlayers(); });
        }
        catch (Exception ex)
        {
            Debug.Log($"에러{ex}");
        }
    }
    #endregion
}