using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
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

public class LobbyManager : IManagerEventInitailize
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
    private const string WAIT_LOBBY_NAME = "WaitLobby";
    private PlayerIngameLoginInfo _currentPlayerInfo;
    private bool _isDoneInitEvent = false;
    private Lobby _currentLobby;
    private bool _isRefreshing = false;
    private bool[] _taskChecker;
    private Coroutine _heartBeatCoroutine = null;
    private Action<bool> _lobbyLoading;
    private Action _initDoneEvent;
    private Action _hostChangeEvent;

    public bool[] TaskChecker => _taskChecker;
    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;
    public event Action<bool> LobbyLoadingEvent
    {
        add
        {

            if (_lobbyLoading != null && _lobbyLoading.GetInvocationList().Contains(value) == true)
                return;

            _lobbyLoading += value;
        }
        remove
        {
            if (_lobbyLoading == null || _lobbyLoading.GetInvocationList().Contains(value) == false)
            {
                Debug.LogWarning($"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                return;
            }
            _lobbyLoading -= value;
        }
    }
    public event Action InitDoneEvent
    {
        add
        {
            if (_initDoneEvent != null && _initDoneEvent.GetInvocationList().Contains(value) == true)
                return;

            _initDoneEvent += value;
        }
        remove
        {
            if (_initDoneEvent == null || _initDoneEvent.GetInvocationList().Contains(value) == false)
            {
                Debug.LogWarning($"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                return;
            }
            _initDoneEvent -= value;
        }
    }
    public event Action HostChageEvent
    {
        add
        {
            if (_hostChangeEvent != null && _hostChangeEvent.GetInvocationList().Contains(value) == true)
                return;

            _hostChangeEvent += value;
        }
        remove
        {
            if (_hostChangeEvent == null || _hostChangeEvent.GetInvocationList().Contains(value) == false)
            {
                Debug.LogWarning($"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                return;
            }
            _hostChangeEvent -= value;
        }
    }


    public string PlayerID => _currentPlayerInfo.Id;
    public bool IsDoneInitEvent { get => _isDoneInitEvent; }
    public void TriggerLobbyLoadingEvent(bool lobbyState)
    {
        _lobbyLoading?.Invoke(lobbyState);
    }
    public async Task<Lobby> GetCurrentLobby()
    {
        if (_currentLobby == null)
            return null;

        _currentLobby = await GetLobbyAsyncCustom(_currentLobby.Id);
        return _currentLobby;
    }
    public async Task<bool> InitLobbyScene()
    {
        bool isalready = false;
        SetLoadingTask(Enum.GetValues(typeof(LoadingProcess)).Length);
        SubscribeSceneEvent();
        try
        {
            await JoinAuthenticationService();
            // Unity Services �ʱ�ȭ
            isalready = await IsAlreadyLogInNickNameinLobby(_currentPlayerInfo.PlayerNickName);
            _taskChecker[(int)LoadingProcess.CheckAlreadyLogInID] = true;
            if (isalready is true)
            {
                Debug.Log("�̹� ���ӵǾ�����");
                return true;
            }
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

        void SetLoadingTask(int taskLength)
        {
            _taskChecker = new bool[taskLength];
            Managers.SceneManagerEx.SetCheckTaskChecker(_taskChecker);
        }
        void SubscribeSceneEvent()
        {
            Managers.RelayManager.SceneLoadInitalizeRelayServer();
            InitalizeVivoxEvent();
            InitalizeLobbyEvent();
            _taskChecker[(int)LoadingProcess.VivoxInitalize] = true;
        }
        async Task JoinAuthenticationService()
        {
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
        }
        async Task<bool> IsAlreadyLogInNickNameinLobby(string usernickName)
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
                        if (player.Data == null && PlayerID == player.Id)//���ε� �����͸� �Ҵ� ���޾�����,�ٽ� �ʱ�ȭ
                        {
                            Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                            return await Utill.RateLimited(() => InitLobbyScene(), 5000); // ��õ�
                        }
                        foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                        {
                            if (player.Data == null)
                                continue;

                            if (PlayerID == player.Id) //�ڱ��ڽ��� �ǳʶٱ�
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
                return await Utill.RateLimited(() => IsAlreadyLogInNickNameinLobby(usernickName)); // ��õ�
            }

            catch (Exception notSetObjectException) when (notSetObjectException.Message.Equals("Object reference not set to an instance of an object"))
            {
                Debug.Log("The Lobby hasnt reference so We Rate Secend");
                return await Utill.RateLimited(() => IsAlreadyLogInNickNameinLobby(usernickName));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to query lobbies: {ex.Message}");
                return false;
            }
        }
    }

    private void SetVivoxTaskCheker()
    {
        _taskChecker[(int)LoadingProcess.VivoxLogin] = true;
        Debug.Log("VivoxLogin����");
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

    private void CheckHostAndSendHeartBeat(Lobby lobby, float interval = 15f)
    {
        try
        {
            Debug.Log($"�κ��� ȣ��Ʈ ID:{lobby.HostId} ���� ���̵�{PlayerID}");
            StopHeartbeat();
            if (lobby.HostId == PlayerID)
            {
                Debug.Log("��Ʈ��Ʈ �̽�");
                _heartBeatCoroutine = Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, interval));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    private async Task JoinRelayServer(Lobby lobby, Func<Lobby, Task> CheckHostAndGuestEvent)
    {
        await CheckHostAndGuestEvent?.Invoke(lobby);
    }
    private async Task RegisteLobbyCallBack(Lobby lobby,
        Action<ILobbyChanges> OnLobbyChangeEvent,
        Action<List<LobbyPlayerJoined>> OnPlayerJoinedEvent = null,
        Action<List<int>> OnPlayerLeftEvent = null)
    {
        LobbyEventCallbacks lobbycallbacks = new LobbyEventCallbacks();
        lobbycallbacks.LobbyChanged += OnLobbyChangeEvent;
        lobbycallbacks.PlayerJoined += OnPlayerJoinedEvent;
        lobbycallbacks.PlayerLeft += OnPlayerLeftEvent;

        try
        {
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbycallbacks);
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

    private async Task CheckHostRelay(Lobby lobby)
    {
        if (lobby.HostId != PlayerID)
            return;

        try
        {
            string joincode = await Managers.RelayManager.StartHostWithRelay(lobby.MaxPlayers);
            Debug.Log(lobby.Name + "�κ��� �̸�");
            await InjectionRelayJoinCodeintoLobby(lobby, joincode);
        }
        catch (LobbyServiceException TimeLimmitException) when (TimeLimmitException.Message.Contains("Rate limit has been exceeded"))
        {
            await Utill.RateLimited(async () => await CheckHostRelay(lobby));
            return;
        }
    }
    private async Task CheckClientRelay(Lobby lobby)
    {
        if (lobby.HostId == PlayerID)
            return;

        try
        {
            string joincode = lobby.Data["RelayCode"].Value;
            await Managers.RelayManager.JoinGuestRelay(joincode);
        }
        catch (KeyNotFoundException exception)
        {
            Debug.Log($"������ �ڵ尡 �������� �ʽ��ϴ�.{exception}");
            await Utill.RateLimited(async () =>
            {
                Lobby currentLobby = await GetLobbyAsyncCustom(lobby.Id);
                await CheckClientRelay(currentLobby);
            });
        }
    }
    private void StopHeartbeat()
    {
        if (_heartBeatCoroutine == null)
            return;


        Managers.ManagersStopCoroutine(_heartBeatCoroutine);
        _heartBeatCoroutine = null;
    }


    public async Task TryJoinLobbyByNameOrCreateWaitLobby()
    {
        if (_currentLobby != null)
            await ExitLobbyAsync(_currentLobby); //������ ���� ����

        try
        {
            CreateLobbyOptions waitLobbyoption = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {WAIT_LOBBY_NAME,new DataObject(DataObject.VisibilityOptions.Public,"PlayerWaitLobbyRoom") }
                }
            };
            await TryJoinLobbyByNameOrCreateLobby(WAIT_LOBBY_NAME, 100, waitLobbyoption);
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

    public async Task<Lobby> AvailableLobby(string lobbyname)
    {

        List<Lobby> fillteredLobbyList = null;
        QueryLobbiesOptions lobbyNameFillter = new QueryLobbiesOptions()
        {
            Filters = new List<QueryFilter>
            {
             new QueryFilter(
                field: QueryFilter.FieldOptions.Name,
                op: QueryFilter.OpOptions.EQ, // EQ�� "���� �̸�"�� ����
                value: lobbyname)        // �� �̸��� ��Ȯ�� ��ġ�ϴ� �κ� �˻�
            }
        };
        QueryResponse queryResponse = await GetQueryLobbiesAsyncCustom(lobbyNameFillter);

        if (queryResponse is null) //�̸����� ��ã�Ҵ�.
        {
            return null;
        }

        fillteredLobbyList = queryResponse.Results;


        foreach (Lobby lobby in fillteredLobbyList)
        {
            if (lobby.Players.Count >= 1)
            {
                return lobby;
            }
        }

        return null;
    }



    private async Task TryJoinLobbyByNameOrCreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions lobbyOption)
    {
        try
        {
            Lobby lobbyResult = await AvailableLobby(lobbyName);
            if (lobbyResult == null)
            {
                Debug.Log("There is not WaitLobby, so Create Wait Lobby");
                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOption);
            }
            else
            {
                Debug.Log("Find WaitLobby, Join to Lobby");
                _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyResult.Id);
            }
            //
            Lobby currentLobby = await GetCurrentLobby();
            CheckHostAndSendHeartBeat(currentLobby);

            Action<ILobbyChanges> waitLobbyDataChangedEvent = null;
            waitLobbyDataChangedEvent += OnLobbyHostChangeEvent;
            waitLobbyDataChangedEvent += NotifyPlayerCreateRoom;
            await JoinLobbyInitalize(currentLobby, waitLobbyDataChangedEvent);
        }
        catch (LobbyServiceException alreayException) when (alreayException.Message.Contains("player is already a member of the lobby"))
        {
            Debug.Log("�÷��̾ �̹� �������Դϴ�. ���������� �������� �õ� �մϴ�.");
            Managers.SceneManagerEx.SetCheckTaskChecker(_taskChecker);
            await InitLobbyScene();
        }
        catch (LobbyServiceException TimeLimmitException) when (TimeLimmitException.Message.Contains("Rate limit has been exceeded"))
        {
            await Utill.RateLimited(() => TryJoinLobbyByNameOrCreateLobby(lobbyName, maxPlayers, lobbyOption));
        }

        catch (KeyNotFoundException keynotFoundExceoption) when (keynotFoundExceoption.Message.Contains("The given key 'RelayCode' was not present in the dictionary"))
        {
            Debug.Log("�������ڵ尡 �����ϴ�. �ٽ� ã���ϴ�");
            await Utill.RateLimited(() => TryJoinLobbyByNameOrCreateLobby(lobbyName, maxPlayers, lobbyOption));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        async void OnLobbyHostChangeEvent(ILobbyChanges lobbyChanges)
        {
            if (lobbyChanges.HostId.Value == PlayerID)
            {
                Lobby currentLobby = await GetCurrentLobby();
                CheckHostAndSendHeartBeat(currentLobby);
            }
        }

        // ��� LobbyChanged �̺�Ʈ���� ȣ��
        async void NotifyPlayerCreateRoom(ILobbyChanges lobbyChanges)
        {
            // PlayerData ������ ������ ���ٸ� ����
            if (lobbyChanges.PlayerData.Value == null)
                return;

            //PlayerData.Value :  Dictionary<int /*player-index*/, LobbyPlayerChanges>
            foreach (KeyValuePair<int, LobbyPlayerChanges> changedData in lobbyChanges.PlayerData.Value)
            {
                LobbyPlayerChanges playerChanges = changedData.Value;

                //�� �÷��̾��� Data �� ������ ���� �ٲ�(Changed) Ű ���
                var dataChanges = playerChanges.ChangedData;

                if (!dataChanges.Changed || dataChanges.Value == null)
                    continue;                                   // ������ ������ ������ �н�

                //LastCreatedRoom �ʵ尡 �ٲ������ Ȯ��
                if (dataChanges.Value.TryGetValue("LastCreatedRoom", out var roomFieldChange) &&
                    roomFieldChange.Changed)                   // �� key ���� ����� ��쿡��
                {
                    // roomFieldChange.Value : PlayerDataObject
                    string newRoomId = roomFieldChange.Value.Value;
                    Debug.Log($"�ٸ� �÷��̾ �� ���� ��������ϴ�!  RoomId = {newRoomId}");

                    // �ʿ��ϴٸ� ��� �� ��� ����
                    await Managers.LobbyManager.ReFreshRoomList();
                }
            }
        }

    }

    public async Task ExitLobbyAsync(Lobby lobby, bool disconnectRelayOption = true)
    {
        if (lobby == null)
            return;

        try
        {
            StopHeartbeat();//��Ʈ��Ʈ ����
            Lobby currentLobby = await GetLobbyAsyncCustom(lobby.Id);//������ �κ� �����;��Ѵ�.
            bool ischeckUserIsHost = currentLobby.HostId == PlayerID;
            bool ischeckUserAloneInLobby = currentLobby.Players.Count <= 1;
            UnScribeRelayCallBack();
            if (disconnectRelayOption == true)
            {
                Managers.RelayManager.ShutDownRelay();
            }
            if (ischeckUserAloneInLobby && ischeckUserIsHost)
            {//���� ȣ��Ʈ�� �κ� ���� ���Ҵٸ� �κ����
                await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
            }
            else
            {
                //�������� ���� ����� ������ �ٸ� ����� �ִµ�, ���� ȣ��Ʈ�ΰ��
                Debug.Log("�κ����� �� �� ������ ����");
                await RemovePlayerData(lobby);
                DeleteRelayCodefromLobby(lobby);
            }
            if (ReferenceEquals(_currentLobby, lobby))
                _currentLobby = null;
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
            Lobby waitLobby = await GetCurrentLobby();

            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            if (_currentLobby != null)
            {
                Debug.Log($"�κ� �������{_currentLobby.Name}");
                await CreateRoomWriteinWaitLobby(waitLobby.Id, PlayerID);
                await ExitLobbyAsync(waitLobby);
            }
            CheckHostAndSendHeartBeat(_currentLobby);
            await JoinLobbyInitalize(_currentLobby, OnRoomLobbyChangeHostEventAsync);
            await JoinRelayServer(_currentLobby, CheckHostRelay);
        }

        catch (Exception e)
        {
            Debug.Log($"An error occurred while creating the room.{e}");
            throw;
        }
        async Task CreateRoomWriteinWaitLobby(string lobbyId, string playerId)
        {
            await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, new UpdatePlayerOptions()
            {
                Data = new Dictionary<string, PlayerDataObject>()
             {
               { "LastCreatedRoom", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,lobbyName) }
             }
            });
        }
    }




    private async void OnRoomLobbyChangeHostEventAsync(ILobbyChanges lobbyChanges)
    {
        if (lobbyChanges.HostId.Value == PlayerID)
        {
            _lobbyLoading?.Invoke(true);
            Lobby currentLobby = await GetCurrentLobby();
            await Managers.LobbyManager.CheckHostRelay(currentLobby);
            CheckHostAndSendHeartBeat(currentLobby);
            _hostChangeEvent?.Invoke();
            _lobbyLoading?.Invoke(false);
        }

        if (lobbyChanges.Data.Value != null && lobbyChanges.Data.Value.TryGetValue("RelayCode", out var relayData) && relayData.Changed)
        {
            _lobbyLoading?.Invoke(true);
            var newCode = relayData.Value;
            Lobby currentLobby = await GetCurrentLobby();
            Debug.Log($"���ο� �������ڵ� {newCode.Value} + ���� �κ� �̸�{currentLobby.Name}");
            await CheckClientRelay(currentLobby);
            _lobbyLoading?.Invoke(false);
        }

    }

    public async Task<Lobby> JoinLobbyByID(string lobbyID, string password = null)
    {
        Lobby preLobby = _currentLobby;
        Lobby nextLobby;
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions() { Password = password };
            nextLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, options);
        }
        catch (LobbyServiceException wrongPw) when (wrongPw.Reason == LobbyExceptionReason.IncorrectPassword)
        {
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

        if (preLobby != null)
            await ExitLobbyAsync(preLobby);

        _currentLobby = nextLobby;
        CheckHostAndSendHeartBeat(_currentLobby);
        await JoinLobbyInitalize(_currentLobby, OnRoomLobbyChangeHostEventAsync);
        await JoinRelayServer(_currentLobby, CheckClientRelay);
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
    }

    private async Task JoinLobbyInitalize(Lobby lobby,
        Action<ILobbyChanges> OnLobbyChangeEvent,
        Action<List<LobbyPlayerJoined>> OnPlayerJoinedEvent = null,
        Action<List<int>> OnPlayerLeftEvent = null)
    {
        _isDoneInitEvent = false;
        try
        {
            await InputPlayerDataIntoLobby(lobby);//�κ� �ִ� player�� �г����߰�
            await Managers.VivoxManager.JoinChannel(lobby.Id);//�񺹽� ����
            await RegisteLobbyCallBack(lobby, OnLobbyChangeEvent, OnPlayerJoinedEvent, OnPlayerLeftEvent);
            _initDoneEvent?.Invoke(); //ȣ���� �Ϸ�Ǿ����� �̺�Ʈ �ݹ�
            _isDoneInitEvent = true;
        }

        catch (Exception ex)
        {
            Debug.LogError($"JoinRoomInitalize �� ���� �߻�: {ex}");
            _isDoneInitEvent = false;
            throw; // ���� ȣ��ο� ���ܸ� ����
        }

    }

    private async Task<Lobby> GetLobbyAsyncCustom(string lobbyId)
    {
        try
        {
            return await LobbyService.Instance.GetLobbyAsync(lobbyId);
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await Utill.RateLimited(() => GetLobbyAsyncCustom(lobbyId));
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
        _lobbyLoading?.Invoke(true);
        List<Lobby> lobbyinPlayerList = await CheckAllLobbyinPlayer();
        foreach (Lobby lobby in lobbyinPlayerList)
        {
            await ExitLobbyAsync(lobby);
            Debug.Log($"{lobby}���� �������ϴ�.");
            StopHeartbeat();
        }
        _lobbyLoading?.Invoke(false);
    }


    private async Task<List<Lobby>> CheckAllLobbyinPlayer()
    {
        //���� �ɼǿ��� ��� �÷��̾ �˻��ϴ� ���� �ɼ��� �����Ƿ� ���� ����
        List<Lobby> lobbyinPlayerList = new List<Lobby>();
        QueryResponse allLobbyResponse = await GetQueryLobbiesAsyncCustom();
        foreach (Lobby lobby in allLobbyResponse.Results)
        {
            if (lobby.Players.Any(player => player.Id == PlayerID))
            {
                lobbyinPlayerList.Add(lobby);
            }
        }
        return lobbyinPlayerList;
    }


    private async Task<string> SignInAnonymouslyAsync()
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

    private async Task InputPlayerDataIntoLobby(Lobby lobby)
    {
        if (lobby == null)
            return;

        Dictionary<string, PlayerDataObject> updatedData = new Dictionary<string, PlayerDataObject>
        {
            { "NickName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, $"{_currentPlayerInfo.PlayerNickName}") },
        };
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, PlayerID, new UpdatePlayerOptions
            {
                Data = updatedData
            });

            Debug.Log($"�κ�ID: {lobby.Id} \t �÷��̾�ID: {PlayerID} ������ �ԷµǾ����ϴ�.");
        }
        catch (Exception error)
        {
            Debug.LogError($"���� �߻�{error}");
        }
    }


    private async Task RemovePlayerData(Lobby lobby)
    {
        Debug.Log($"�κ�ID{lobby.Id} \t �÷��̾�ID{PlayerID} ������ ���ŵǾ����ϴ�.");
        await LobbyService.Instance.RemovePlayerAsync(lobby.Id, PlayerID);
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
        Managers.VivoxManager.VivoxDoneLoginEvent += SetVivoxTaskCheker;
    }
    public void InitalizeLobbyEvent()
    {
        Managers.SocketEventManager.LogoutAllLeaveLobbyEvent += LogoutAndAllLeaveLobby;
    }
    public void SubScribeRelayCallBack()
    {
        Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback -= OnRelayClientDisconnected;
        Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback += OnRelayClientDisconnected;
    }

    public void UnScribeRelayCallBack()
    {
        Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback -= OnRelayClientDisconnected;
    }
    private async void OnRelayClientDisconnected(ulong clientId)
    {
        await DisconnectClientToRelay(clientId);
    }

    public async Task ReFreshRoomList()
    {
        if (_currentLobby == null)
            return;


        if (_isRefreshing || Managers.UI_Manager.Try_Get_Scene_UI(out UI_Room_Inventory room_inventory_ui) == false)
        {
            return;
        }

        _isRefreshing = true;

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
            _isRefreshing = false;
            throw;
        }
        _isRefreshing = false;
    }

    private void CreateRoomInitalize(Lobby lobby)
    {

        if (Managers.UI_Manager.Try_Get_Scene_UI(out UI_Room_Inventory room_inventory_ui) == false)
            return;

        UI_Room_Info_Panel infoPanel = Managers.UI_Manager.MakeSubItem<UI_Room_Info_Panel>(room_inventory_ui.Room_Content);
        infoPanel.SetRoomInfo(lobby);
    }

    /// <summary>
    /// �� �޼���� �κ񿡼� �÷��̾ ������ �� �ȿ� �� �ִ� ����鸸 ȣ��Ǿ���
    /// �� ����鳢�� ���ؼ� ���� �κ� ���� ȣ��Ʈ�� ȣ��Ʈ ���̱׷��̼� �۾��� �ؾ���
    /// </summary>
    /// <returns></returns>
    private async Task DisconnectClientToRelay(ulong clientid)
    {
        Debug.Log(clientid + "�ʰ� ȣ����??");
        _lobbyLoading?.Invoke(true);
        Debug.Log($"{_currentLobby.Name}");
        _currentLobby = await GetCurrentLobby();
        if (_heartBeatCoroutine == null)
        {
            CheckHostAndSendHeartBeat(_currentLobby);
            await JoinLobbyInitalize(_currentLobby, OnRoomLobbyChangeHostEventAsync);
            await JoinRelayServer(_currentLobby, CheckHostRelay);
        }
        //await ReFreshRoomList();
        _lobbyLoading?.Invoke(false);
    }

    public async Task LoadingPanel(Func<Task> process)
    {
        _lobbyLoading?.Invoke(true);
        await process.Invoke();
        _lobbyLoading?.Invoke(false);
    }
    public void DeleteRelayCodefromLobby(Lobby lobby)
    {
        if (lobby.HostId == PlayerID)
        {
            lobby.Data.Remove("RelayCode");
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
                Unity.Services.Lobbies.Models.Player hostPlayer = lobby.Players.FirstOrDefault(player => player.Id == lobby.HostId);

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