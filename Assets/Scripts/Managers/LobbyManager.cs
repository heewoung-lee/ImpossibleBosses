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

    private const string LOBBYID = "WaitLobbyRoom307";
    private PlayerIngameLoginInfo _currentPlayerInfo;
    private bool _isDoneInitEvent = false;
    private Lobby _currentLobby;
    private bool isRefreshing = false;
    private bool isalready = false;
    private bool[] _taskChecker;
    private Coroutine _heartBeatCoroutine = null;

    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;
    public Action InitDoneEvent;
    public Action<string> PlayerAddDataInputEvent;
    public Action<int> PlayerDeleteEvent;
    public Action<bool> LobbyLoading;
    public string PlayerID => _currentPlayerInfo.Id;
    public Lobby CurrentLobby => _currentLobby;
    public bool IsDoneInitEvent { get => _isDoneInitEvent; }
    public bool[] TaskChecker => _taskChecker;

    public async Task<bool> InitLobbyScene()
    {
        _taskChecker = new bool[Enum.GetValues(typeof(LoadingProcess)).Length];
        LoadingScene.SetCheckTaskChecker(_taskChecker);
        Managers.RelayManager.SceneLoadInitalizeRelayServer();
        InitalizeVivoxEvent();
        InitalizeLobbyEvent();
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
            await SignInAnonymouslyAsync();
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

    public async Task CheckPlayerHostAndClient(Lobby lobby, Func<Lobby, Task> CheckHostAndGuestEvent, float interval = 15f)
    {
        try
        {
            if (lobby == null || _currentPlayerInfo.Id == null)
                return;

            CheckHostAndSendHeartBeat(lobby, interval);
            Managers.RelayManager.ShutDownRelay();
            await CheckHostAndGuestEvent?.Invoke(lobby);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void CheckHostAndSendHeartBeat(Lobby lobby, float interval = 15f)
    {
        StopHeartbeat();
        if (lobby.HostId == _currentPlayerInfo.Id)
        {
            Debug.Log("��Ʈ��Ʈ �̽�");
            _heartBeatCoroutine = Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, interval));
        }
    }
    private async Task CheckHostOrClientRelay(Lobby lobby)
    {
        await CheckHostRelay(lobby);
        await CheckClientRelay(lobby);
    }

    private async Task CheckHostRelay(Lobby lobby)
    {
        if (lobby.HostId != _currentPlayerInfo.Id)
            return;


        try
        {
            string joincode = await Managers.RelayManager.StartHostWithRelay(lobby.MaxPlayers);
            await InjectionRelayJoinCodeintoLobby(lobby, joincode);
        }
        catch(LobbyServiceException TimeLimmitException) when(TimeLimmitException.Message.Contains("Rate limit has been exceeded"))
        {
            await Utill.RateLimited(async () => await CheckHostRelay(lobby));
            return;
        }
    }
    private async Task CheckClientRelay(Lobby lobby)
    {
        if (lobby.HostId == _currentPlayerInfo.Id)
            return;

        try
        {
            string joincode = lobby.Data["RelayCode"].Value;
            await Managers.RelayManager.JoinGuestRelay(joincode);
        }
        catch (KeyNotFoundException exception)
        {
            Debug.Log($"������ �ڵ尡 �������� �ʽ��ϴ�.{exception}");
            await Utill.RateLimited(async () => await InitLobbyScene());
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
            await LeaveLobbyAndDisconnectRelay(_currentLobby); //������ ���� ����
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
            Debug.Log("���⼭ ������ �߻�" + e);
        }
    }

    public async Task TryJoinLobbyByNameOrCreateLobby(string lobbyName, int MaxPlayers, CreateLobbyOptions lobbyOption)
    {
        try
        {
            _currentLobby = await LobbyService.Instance.CreateOrJoinLobbyAsync(LOBBYID, lobbyName, MaxPlayers, lobbyOption);
            await JoinLobbyInitalize(_currentLobby);
            await CheckPlayerHostAndClient(_currentLobby, CheckHostOrClientRelay);
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

    public async Task LeaveLobbyAndDisconnectRelay(Lobby lobby)
    {
        try
        {
            if (lobby != null)
            {
                await RemovePlayerData(lobby);
                Managers.RelayManager.UnSubscribeCallBackEvent();
                Managers.RelayManager.ShutDownRelay();
                DeleteRelayCodefromLobby();
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

    public async Task LeaveCurrentLobby()
    {
        await LeaveLobby(_currentLobby);
    }
    public async Task LeaveLobby(Lobby lobby)
    {
        if (lobby != null)
        {
            await RemovePlayerData(lobby);
            DeleteRelayCodefromLobby();
            StopHeartbeat();
        }
    }
    public async Task CreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions options)
    {
        try
        {
            await LeaveLobbyAndDisconnectRelay(_currentLobby);
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            if (_currentLobby != null)
                Debug.Log($"�κ� �������{_currentLobby.Name}");

            await JoinLobbyInitalize(_currentLobby);
            await CheckPlayerHostAndClient(_currentLobby, CheckHostRelay);
        }

        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
    }


    public async Task<Lobby> CreateLobbyID(string lobbyID, string lobbyName, int maxPlayers, CreateLobbyOptions options = null)
    {
        try
        {
            if (_currentLobby != null)
            {
                await LeaveLobbyAndDisconnectRelay(_currentLobby);
            }
            Lobby lobby = await LobbyService.Instance.CreateOrJoinLobbyAsync(lobbyID, lobbyName, maxPlayers, options);
            await JoinLobbyInitalize(lobby);
            await CheckPlayerHostAndClient(lobby, CheckHostRelay);
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            return null;
            throw;
        }
    }
    public async Task<Lobby> JoinLobbyByID(string lobbyID, string password = null)
    {
        Lobby waitLobby = _currentLobby;
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
            _currentLobby = waitLobby;
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
        await LeaveLobbyAndDisconnectRelay(waitLobby);
        await JoinLobbyInitalize(_currentLobby);
        await CheckPlayerHostAndClient(_currentLobby, CheckClientRelay);

        return _currentLobby;
    }
    private async Task InjectionRelayJoinCodeintoLobby(Lobby lobby, string joincode)
    {
        
            if (joincode == null || lobby == null)
            {
                Debug.Log($"Data has been NULL, is Check Lobby Null?: {lobby.Equals(null)} is Check JoinCode Null? {lobby.Equals(null)}");
                return;
            }
            _currentLobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>
                {
                   {"RelayCode",new DataObject(DataObject.VisibilityOptions.Public,joincode)}
                }
            });
            await Managers.VivoxManager.SendSystemMessageAsync("ȣ��Ʈ�� ����Ǿ����ϴ�.���ΰ�ħ �մϴ�.");
    }

    private async Task JoinLobbyInitalize(Lobby lobby)
    {
        _isDoneInitEvent = false;
        try
        {
            Task inputplayerDataTask = InputPlayerDataIntoLobby(lobby);//�κ� �ִ� player�� �г����߰�
            Task vivoxChanelTask = Managers.VivoxManager.JoinChannel(lobby.Id);

            await Task.WhenAll(inputplayerDataTask, vivoxChanelTask);
            InitalizeRelayEvent();
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

    public async Task<Lobby> GetLobbyAsyncCustom(string lobbyID)
    {
        try
        {
            return await LobbyService.Instance.GetLobbyAsync(lobbyID);
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            Debug.Log($"Stack Trace{System.Environment.StackTrace}");
            return await Utill.RateLimited(() => GetLobbyAsyncCustom(lobbyID));
        }
    }

    public async Task<(bool, Lobby)> TryGetLobbyAsyncCustom(string lobbyId)
    {
        try
        {
            Lobby lobby = await GetLobbyAsyncCustom(lobbyId);
            return (true, lobby);
        }
        catch (LobbyServiceException ex) when (ex.Reason == LobbyExceptionReason.LobbyNotFound)
        {
            return (false, null);
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



    public async Task LeaveAllLobby()
    {
        LobbyLoading?.Invoke(true);
        List<Lobby> lobbyinPlayerList = await CheckAllLobbyinPlayer();
        foreach (Lobby lobby in lobbyinPlayerList)
        {
            await LeaveLobbyAndDisconnectRelay(lobby);
            Debug.Log($"{lobby}���� �������ϴ�.");
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
            if (lobby.Players.Any(player => player.Id == _currentPlayerInfo.Id))
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
                    if (player.Data == null && _currentPlayerInfo.Id == player.Id)//���ε� �����͸� �Ҵ� ���޾�����,�ٽ� �ʱ�ȭ
                    {
                        Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                        return await Utill.RateLimited(() => InitLobbyScene(), 5000); // ��õ�
                    }
                    foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                    {
                        if (player.Data == null)
                            continue;

                        if (_currentPlayerInfo.Id == player.Id) //�ڱ��ڽ��� �ǳʶٱ�
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

        catch (Exception notSetObjectException) when (notSetObjectException.Message.Equals("Object reference not set to an instance of an object"))
        {
            Debug.Log("The Lobby hasnt reference so We Rate Secend");
            return await Utill.RateLimited(() => IsAlreadyLogInID(usernickName));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to query lobbies: {ex.Message}");
            return false;
        }
    }

    private async Task InputPlayerDataIntoLobby(Lobby lobby)
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
        Debug.Log($"�κ�ID{lobby.Id} \t �÷��̾�ID{_currentPlayerInfo.Id} ������ ���ŵǾ����ϴ�.");
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
        Managers.VivoxManager.VivoxDoneLoginEvent -= SetVivoxTaskCheker;
        Managers.VivoxManager.VivoxDoneLoginEvent += SetVivoxTaskCheker;
   
    }
    public void InitalizeLobbyEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutAndAllLeaveLobby;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutAndAllLeaveLobby;
    }
    public void InitalizeRelayEvent()
    {
        Managers.RelayManager.DisconnectPlayerAsyncEvent -= DisconnectPlayer;
        Managers.RelayManager.DisconnectPlayerAsyncEvent += DisconnectPlayer;
        Managers.RelayManager.InitalizeRelayServer();
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
            Debug.Log($"�����߻�:{e}");
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


    public async Task DisconnectPlayer()
    {
        LobbyLoading?.Invoke(true);
        await ReFreshRoomList();
        _currentLobby = await GetLobbyAsyncCustom(_currentLobby.Id);
        if (_heartBeatCoroutine == null)
        {
            await CheckPlayerHostAndClient(_currentLobby, CheckHostRelay);
        }
        LobbyLoading?.Invoke(false);
    }

    public async Task DisconnetPlayerinRoom()
    {
        LobbyLoading?.Invoke(true);
        _currentLobby = await GetLobbyAsyncCustom(_currentLobby.Id);
        if (_heartBeatCoroutine == null)
        {
            await CheckPlayerHostAndClient(_currentLobby, CheckHostRelay);
        }
        LobbyLoading?.Invoke(false);
    }

    public async Task RefreshClientPlayer()
    {
        LobbyLoading?.Invoke(true);
        _currentLobby = await GetLobbyAsyncCustom(_currentLobby.Id);
        await CheckClientRelay(_currentLobby);
        LobbyLoading?.Invoke(false);
    }

    public async Task<bool> isCheckLobbyInClientPlayer(string LobbyId, string playerID)
    {
        Lobby lobby = await GetLobbyAsyncCustom(LobbyId);

        foreach (Player player in lobby.Players)
        {
            if (lobby.HostId == playerID || player.Id != playerID)
                continue;

            if (player.Id == playerID)
                return true;
        }
        return false;
    }


    public async Task ExecuteSystemMessageAction(string messageChannel, string systemMessageText, string detectionText, Func<Task> executeMethod)
    {
        if (await isCheckLobbyInClientPlayer(messageChannel, Managers.LobbyManager.PlayerID)
            && systemMessageText.Contains(detectionText))
        {
            Debug.Log("Ŭ���̾�Ʈ�鿡�Ը� ����");
            await executeMethod.Invoke();
        }
    }

    public async Task LoadingPanel(Func<Task> process)
    {
        LobbyLoading?.Invoke(true);
        await process.Invoke();
        LobbyLoading?.Invoke(false);
    }
    public void LoadingPanel(Action process)
    {
        LobbyLoading.Invoke(true);
        process.Invoke();
        LobbyLoading.Invoke(false);
    }

    public void DeleteRelayCodefromLobby()
    {
        if (_currentLobby.Players.Count == 1)
        {
            Debug.Log("�ѻ���� ����");
            _currentLobby.Data.Remove("RelayCode");
        }

    }
    #region TestDebugCode
    public void SetPlayerLoginInfo(PlayerIngameLoginInfo info)
    {
        _currentPlayerInfo = info;
    }
    public void ShowLobbyData()
    {
        foreach (var data in _currentLobby.Data)
        {
            Debug.Log($"{data.Key}�� ���� {data.Value.Value}");
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

                Debug.Log($"���� �κ��̸�: {lobby.Name} �κ�ID: {lobby.Id} ȣ��Ʈ�г���: {hostPlayer.Data["NickName"].Value} �κ�ȣ��Ʈ: {lobby.HostId} ");
                Debug.Log($"-----------------------------------");
                foreach (var player in lobby.Players)
                {
                    Debug.Log($"�÷��̾� ���̵�: {player.Id} �÷��̾� �г���:{player.Data["NickName"].Value}");
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
            Debug.Log($"����{ex}");
        }
    }
    #endregion
}