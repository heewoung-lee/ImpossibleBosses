using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Player = Unity.Services.Lobbies.Models.Player;



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
    private const string LOBBYID = "WaitLobbyRoom50";
    private PlayerIngameLoginInfo _currentPlayerInfo;
    private bool _isDoneInitEvent = false;
    private string _playerID;
    private Lobby _currentLobby;
    private bool isRefreshing = false;
    private bool isalready = false;
    private UI_Room_Inventory _ui_Room_Inventory;
    private ILobbyEvents _lobbyEvents;
    private Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> _playersJoinLobbyDict;
    private LobbyEventCallbacks _callBackEvent;
    private bool[] _taskChecker;
    private HashSet<LobbyPlayerJoined> _playerJoined = null;
    private Coroutine _heartBeatCoroutine = null;
    private UI_LobbyChat _ui_LobbyChat = null;

    public string GetLobbyID => LOBBYID;

    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;
    public Action InitDoneEvent;
    public Action<string> PlayerAddDataInputEvent;
    public Action<int> PlayerDeleteEvent;

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
        _taskChecker[(int)LoadingProcess.VivoxInitalize] = true;
        try
        {
            // Unity Services �ʱ�ȭ
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
                Debug.Log("�̹� ���ӵǾ�����");
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

    public void CheckHostAndSendsHeartBeat(Lobby lobby, float interval = 20f)
    {
        if (lobby == null || _playerID == null)
            return;

        StopHeartbeat();
        if (_currentLobby.HostId == _playerID)
        {
            Debug.Log("��Ʈ��Ʈ �̽�");
            StartHeartbeat(lobby, interval);
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
    private void StartHeartbeat(Lobby lobby, float interval = 20)
    {
        _heartBeatCoroutine = Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, interval));
    }

    public async Task TryJoinLobbyByNameOrCreateWaitLobby()
    {
        if (_currentLobby != null)
            await LeaveLobby(_currentLobby); //������ ���� ����

        try
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

        catch (LobbyServiceException playerNotFound) when (playerNotFound.Message.Contains("player not found"))
        {
            Debug.Log("Player Not Found");
            await InitLobbyScene();
            return;
        }
        catch (Exception e)
        {
            Debug.Log("���⼭ ������ �߻�" + e);
        }
    }

    public async Task TryJoinLobbyByNameOrCreateLobby(string lobbyName, int MaxPlayers, CreateLobbyOptions lobbyOption)
    {
        try
        {
            _currentLobby = await LobbyService.Instance.CreateOrJoinLobbyAsync(LOBBYID, lobbyName, MaxPlayers, lobbyOption);
            CheckHostAndSendsHeartBeat(_currentLobby);
            await JoinLobbyInitalize(_currentLobby);

            Debug.Log("�κ����: "+_currentLobby.Version);
        }
        catch (LobbyServiceException alreayException) when (alreayException.Message.Contains("player is already a member of the lobby"))
        {
            Debug.Log("�÷��̾ �̹� �������Դϴ�. ���������� �������� �õ� �մϴ�.");
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
            Debug.Log("��й�ȣ�� Ʋ�Ƚ��ϴ�!");
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
            if (_lobbyEvents != null)
            {
                await _lobbyEvents.UnsubscribeAsync();
                _lobbyEvents = null;
            }
        }
        catch (System.ObjectDisposedException disposedException)
        {
            Debug.Log($"�̹� ��ü�� ���ŵǾ����ϴ�.{disposedException.Message}");
        }
        catch (Exception e)
        {
            Debug.Log($"LeaveLobby �޼��� �ȿ����� ����{e}");
        }
    }
    public async Task CreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions options)
    {
        try
        {
            await LeaveLobby(_currentLobby);
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            if (_currentLobby != null)
                Debug.Log($"�κ� �������{_currentLobby.Name}");
            await JoinLobbyInitalize(_currentLobby);
            CheckHostAndSendsHeartBeat(_currentLobby);
        }

        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
    }

    private async Task JoinLobbyInitalize(Lobby lobby)
    {
        _isDoneInitEvent = false;
        try
        {
            await InputPlayerData(lobby);
            await Managers.VivoxManager.JoinChannel(lobby.Id);
            await RegisterEvents(lobby);
            //await ReRegisterEventCallback();
            InitDoneEvent?.Invoke();
            _isDoneInitEvent = true;
        }

        catch (Exception ex)
        {
            Debug.LogError($"JoinRoomInitalize �� ���� �߻�: {ex}");
            _isDoneInitEvent = false;
            throw; // ���� ȣ��ο� ���ܸ� ����
        }

    }
    public async Task RegisterEvents(Lobby lobby)
    {
        try
        {
            _callBackEvent = new LobbyEventCallbacks();
            _callBackEvent.PlayerDataAdded -= (playerdaData) => JoinLobbyPlayerDataAdded(playerdaData);
            _callBackEvent.PlayerDataAdded += (playerdaData) => JoinLobbyPlayerDataAdded(playerdaData);
            //_callBackEvent.PlayerJoined -= PlayerJoinedEvent;
            //_callBackEvent.PlayerJoined += PlayerJoinedEvent;
            _callBackEvent.PlayerLeft -= async (leftPlayerlist) => await PlayerLeftEvent(leftPlayerlist);
            _callBackEvent.PlayerLeft += async (leftPlayerlist) =>await PlayerLeftEvent(leftPlayerlist);
            _callBackEvent.LobbyChanged -= CallBackEvent_LobbyChanged;
            _callBackEvent.LobbyChanged += CallBackEvent_LobbyChanged;
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

        catch (Exception ex)
        {
            Debug.Log($"{ex}");
        }

    }

    private void CallBackEvent_LobbyChanged(ILobbyChanges lobbyChanges)
    {
        Debug.Log("CallBackEvent_LobbyChanged ȣ��");

        if (lobbyChanges.HostId.Value == null)
            return;

        if (_playerID == lobbyChanges.HostId.Value)
        {
            Debug.Log($"���� ����");
            StopHeartbeat();
            StartHeartbeat(_currentLobby);
        }
    }
    private async Task PlayerLeftEvent(List<int> leftPlayerlist)
    {
        Debug.Log("PlayerLeftEvent ȣ��");

        foreach (int leftPlayerIndex in leftPlayerlist)
        {
            Debug.Log($"{leftPlayerIndex}��° �÷��̾ �κ񿡼� �������ϴ�.");
            PlayerDeleteEvent?.Invoke(leftPlayerIndex);
        }
        await ReFreshRoomList();
    }
    public async void PlayerJoinedEvent(List<LobbyPlayerJoined> joined)
    {
        await ReFreshRoomList();
    }
    private void JoinLobbyPlayerDataAdded(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> dictionary)
    {

        Debug.Log("JoinLobbyPlayerDataAdded ȣ��");
        foreach (int keyIndex in dictionary.Keys)
        {
            Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>> joinPlayerDict = dictionary[keyIndex];
            ChangedOrRemovedLobbyValue<PlayerDataObject> playerValue = joinPlayerDict["NickName"];

            if (playerValue.Equals(default))
            {
                Debug.Log("NickName �����Ͱ� �����ϴ�.");
                return;
            }
            if (playerValue.Value.Value != _currentPlayerInfo.PlayerNickName)
                PlayerAddDataInputEvent?.Invoke(playerValue.Value.Value);
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
        List<Lobby> lobbyinPlayerList = await CheckAllLobbyinPlayer();
        foreach (Lobby lobby in lobbyinPlayerList)
        {
            await LeaveLobby(lobby);
            Debug.Log($"{lobby}���� �������ϴ�.");
            StopHeartbeat();
        }
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
            Debug.Log($"�÷��̾� ID �������{playerID}");
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
                    if (player.Data == null && _playerID == player.Id)//���ε� �����͸� �Ҵ� ���޾�����,�ٽ� �ʱ�ȭ
                    {
                        Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                        return await Utill.RateLimited(() => InitLobbyScene(), 5000); // ��õ�
                    }
                    foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                    {
                        if (player.Data == null)
                            continue;

                        if (_playerID == player.Id) //�ڱ��ڽ��� �ǳʶٱ�
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
            return await Utill.RateLimited(() => IsAlreadyLogInID(usernickName)); // ��õ�
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

            Debug.Log($"�κ�ID: {lobby.Id} \t �÷��̾�ID: {_currentPlayerInfo.Id} ������ �ԷµǾ����ϴ�.");
        }
        catch (Exception error)
        {
            Debug.LogError($"���� �߻�{error}");
        }

    }


    private async Task RemovePlayerData(Lobby lobby)
    {
        Debug.Log($"�κ�ID{lobby.Id} \t �÷��̾�ID{_playerID} ������ ���ŵǾ����ϴ�.");
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
            Debug.Log($"Failed to remove player from lobby: {ex.Message} �ٽ� �õ���");
            await Utill.RateLimited(() => LogoutAndAllLeaveLobby());
        }
        catch (Exception ex)
        {
            Debug.Log($"�����߻�: {ex}");
        }
        // ����� ���� �α׾ƿ�
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
    public async Task ShowUpdatedLobbyPlayers()
    {
        try
        {
            QueryResponse lobbies = await GetQueryLobbiesAsyncCustom();
            foreach (var lobby in lobbies.Results)
            {
                Player hostPlayer = lobby.Players.FirstOrDefault(player => player.Id == lobby.HostId);

                Debug.Log($"���� �κ��̸�: {lobby.Name} �κ�ID: {lobby.Id} ȣ��Ʈ�г���: {hostPlayer.Data["NickName"].Value} �κ�ȣ��Ʈ: {lobby.HostId} ");
                Debug.Log($"-----------------------------------");
                foreach (var player in lobby.Players)
                {
                    Debug.Log($"�÷��̾� ���̵�: {player.Id} �÷��̾� �г���:{player.Data["NickName"].Value}");
                }
                Debug.Log($"-----------------------------------");
            }

            Debug.Log($"����κ�{_currentLobby.Id} �κ� �̸�{_currentLobby.Name}");
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RequestTimeOut)
        {
            Debug.LogError($"RequestTimeOut");
            await Utill.RateLimited(async () => { await ShowUpdatedLobbyPlayers(); });
        }
        catch (Exception ex)
        {
            Debug.Log($"����{ex}");
        }
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


    public async Task ReRegisterEventCallback()
    {

        if (_lobbyEvents != null)
        {
            await _lobbyEvents.UnsubscribeAsync();
            _lobbyEvents = null;
        }
        await RegisterEvents(_currentLobby);
    }
}