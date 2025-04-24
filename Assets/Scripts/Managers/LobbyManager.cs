using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


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
    private Action _hostChangeEvent;
    private Action<bool> _lobbyLoading;
    private Action _initDoneEvent;

    public bool[] TaskChecker => _taskChecker;
    public PlayerIngameLoginInfo CurrentPlayerInfo => _currentPlayerInfo;

    public event Action HostChangeEvent
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

    public string PlayerID => _currentPlayerInfo.Id;
    public bool IsDoneInitEvent { get => _isDoneInitEvent; }
    public void TriggerLobbyLoadingEvent(bool lobbyState)
    {
        _lobbyLoading?.Invoke(lobbyState);
    }
    public async Task<Lobby> GetCurrentLobby()
    {
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

    public async Task CheckPlayerHostAndClient(Lobby lobby, Func<Lobby, Task> CheckHostAndGuestEvent, float interval = 15f)
    {
        try
        {
            if (lobby == null || _currentPlayerInfo.Id == null)
                return;

            CheckHostAndSendHeartBeat(lobby, interval);
            await JoinLobbyInitalize(lobby);
            await CheckHostAndGuestEvent?.Invoke(lobby);
            JoinRelayInitalize();
            RegisteLobbyCallBack(lobby);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void JoinRelayInitalize()
    {
        SubScribeRelayCallBack();//������ �̺�Ʈ ����
    }
    private void CheckHostAndSendHeartBeat(Lobby lobby, float interval = 15f)
    {
        try
        {
            Debug.Log($"�κ��� ȣ��Ʈ ID:{lobby.HostId} ���� ���̵�{_currentPlayerInfo.Id}");
            StopHeartbeat();
            if (lobby.HostId == _currentPlayerInfo.Id)
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
    private async Task CheckHostOrClientRelay(Lobby lobby)
    {
        await CheckHostRelay(lobby);
        await CheckClientRelay(lobby);
    }


    private async void RegisteLobbyCallBack(Lobby lobby)
    {
        Lobby currentLobby = await GetLobbyAsyncCustom(lobby.Id);
        LobbyEventCallbacks lobbycallbacks = new LobbyEventCallbacks();
        lobbycallbacks.LobbyChanged += OnLobbyChange;
        lobbycallbacks.PlayerJoined += OnPlayerJoined;
        lobbycallbacks.PlayerLeft += OnPlayerLeft;

        try
        {
           await LobbyService.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, lobbycallbacks);
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{currentLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                default: throw;
            }
        }

        void OnLobbyChange(ILobbyChanges changes)
        {
            if(changes.HostId.Value == _currentPlayerInfo.Id)
            {
                Debug.Log("���� ȣ��Ʈ�� �Ǿ����ϴ�.");
            }
        }
        void OnPlayerJoined(List<LobbyPlayerJoined> list)
        {
            Debug.Log("�÷��̾ �����߽��ϴ�");
        }

        void OnPlayerLeft(List<int> list)
        {
            Debug.Log("�÷��̾ �������ϴ�");
        }

    }

    private async Task CheckHostRelay(Lobby lobby)
    {
        if (lobby.HostId != _currentPlayerInfo.Id)
            return;

        try
        {
            string joincode = await Managers.RelayManager.StartHostWithRelay(lobby.MaxPlayers);
            Debug.Log(lobby.Name + "�κ��� �̸�");
            await InjectionRelayJoinCodeintoLobby(lobby, joincode);
            _hostChangeEvent?.Invoke();
        }
        catch (LobbyServiceException TimeLimmitException) when (TimeLimmitException.Message.Contains("Rate limit has been exceeded"))
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
            await ExitLobbyAsync(await GetCurrentLobby()); //������ ���� ����

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

    public async Task<(bool, Lobby)> AvailableLobby(string lobbyname)
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
            return (false, null);
        }

        fillteredLobbyList = queryResponse.Results;

        fillteredLobbyList = fillteredLobbyList.Where(lobby=>lobby.Players.Count >= 1).ToList();

        foreach (Lobby lobby in fillteredLobbyList)
        {
            string joincode = lobby.Data["RelayCode"].Value;

            if (String.IsNullOrEmpty(joincode))//���� �ڵ� �ִ��� Ȯ��.
                continue;


            if(await Managers.RelayManager.IsValidRelayJoinCode(joincode) == true) //�����ڵ尡 ��ȿ���� Ȯ��
            {
                return (true, lobby);
            }
        }
        return (false, null);
    }



    public async Task TryJoinLobbyByNameOrCreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions lobbyOption)
    {
        try
        {
            //_currentLobby = await LobbyService.Instance.CreateOrJoinLobbyAsync(LOBBYID, lobbyName, maxPlayers, lobbyOption);
            (bool, Lobby) lobbyResult = await AvailableLobby(lobbyName);
            if (lobbyResult.Item1 == false)
            {
                Debug.Log("There is not WaitLobby, so Create Wait Lobby");
                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOption);
            }
            else
            {
                Debug.Log("Find WaitLobby, Join to Lobby");
                _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyResult.Item2.Id);
            }
            //
            await CheckPlayerHostAndClient(await GetCurrentLobby(), CheckHostOrClientRelay);
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
    }

    public async Task ExitLobbyAsync(Lobby lobby, bool disconnectRelayOption = true)
    {
        if (lobby == null)
            return;

        try
        {
            StopHeartbeat();//��Ʈ��Ʈ ����
            Lobby currentLobby = await GetLobbyAsyncCustom(lobby.Id);//������ �κ� �����;��Ѵ�.
            bool ischeckUserIsHost = currentLobby.HostId == _currentPlayerInfo.Id;
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
            await ExitLobbyAsync(await GetCurrentLobby());
            //���� ��ϵ� �ݹ� ���������� Ȯ��.

            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            if (_currentLobby != null)
                Debug.Log($"�κ� �������{_currentLobby.Name}");

            await CheckPlayerHostAndClient(await GetCurrentLobby(), CheckHostRelay);
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
                await ExitLobbyAsync(await GetCurrentLobby());
            }
            Lobby lobby = await LobbyService.Instance.CreateOrJoinLobbyAsync(lobbyID, lobbyName, maxPlayers, options);
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
        await CheckPlayerHostAndClient(await GetCurrentLobby(), CheckClientRelay);
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
            await InputPlayerDataIntoLobby(lobby);//�κ� �ִ� player�� �г����߰�
            Debug.Log("����ä��ȣ��");
            await Managers.VivoxManager.JoinChannel(lobby.Id);//�񺹽� ����
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

    public async Task<Lobby> GetLobbyAsyncCustom(string lobbyId)
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
        await DisconnectPlayer(clientId);
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

    public void CreateRoomInitalize(Lobby lobby)
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
    public async Task DisconnectPlayer(ulong clientid)
    {

        Debug.Log(clientid + "�ʰ� ȣ����??");
        _lobbyLoading?.Invoke(true);
        Debug.Log($"{_currentLobby.Name}");
        _currentLobby = await GetCurrentLobby();
        if (_heartBeatCoroutine == null)
        {
            await CheckPlayerHostAndClient(await GetCurrentLobby(), CheckHostRelay);
        }
        await ReFreshRoomList();
        _lobbyLoading?.Invoke(false);
    }

    public async Task JoinRelayOfNewHost()
    {
        _lobbyLoading?.Invoke(true);
        _currentLobby = await GetCurrentLobby();
        await CheckClientRelay(_currentLobby);
        _lobbyLoading?.Invoke(false);
    }

    public async Task<bool> isCheckLobbyInClientPlayer(string LobbyId, string playerID)
    {
        Lobby lobby = await GetLobbyAsyncCustom(LobbyId);

        if (lobby == null) return false;

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
        _lobbyLoading?.Invoke(true);
        await process.Invoke();
        _lobbyLoading?.Invoke(false);
    }
    public void DeleteRelayCodefromLobby(Lobby lobby)
    {
        if (lobby.HostId == _currentPlayerInfo.Id)
        {
            Debug.Log("���� �����̴� ������ �ڵ带 ���ﲲ");
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