
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
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
    private const string LOBBYID = "WaitLobbyRoom";
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
    public Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> PlayersJoinLobbyDict
    {
        get
        {
            if (_playersJoinLobbyDict == null)
            {
                _playersJoinLobbyDict = new Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> ();
            }
            return _playersJoinLobbyDict;
        }
    }
    public bool[] TaskChecker => _taskChecker;

    public async Task<bool> InitLobbyScene()
    {

        _taskChecker = new bool[Enum.GetValues(typeof(LoadingProcess)).Length];
        LoadingScene.SetCheckTaskChecker(_taskChecker);
        Managers.VivoxManager.VivoxDoneLoginEvent += () => _taskChecker[(int)LoadingProcess.VivoxLogin] = true;
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
                return true;
            }
            _taskChecker[(int)LoadingProcess.CheckAlreadyLogInID] = true;

            ///������� ���� ����;
            await TryJoinLobbyByNameOrCreateWaitLobby();
            _taskChecker[(int)LoadingProcess.TryJoinLobby] = true;
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
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }

    }
    public async Task TryJoinLobbyByNameOrCreateWaitLobby()
    {
        if (_currentLobby != null)
            await LeaveLobby(_currentLobby);

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
            await JoinLobbyInitalize(_currentLobby);
            if (_currentLobby.HostId == _playerID)
            {
                //TODO: _playerID�� ���� �Ҵ��� �ȵ� �Ҵ��� ���� �÷��ߵ�
                Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(_currentLobby.Id, 30f));
            }
        }
        catch (LobbyServiceException alreayException) when (alreayException.Message.Contains("player is already a member of the lobby"))
        {
            Debug.Log("�÷��̾ �̹� �������Դϴ�. ���������� �������� �õ� �մϴ�.");
            await LeaveAllLobby();
            await TryJoinLobbyByNameOrCreateLobby(lobbyName, MaxPlayers, lobbyOption);
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
       catch(Exception e)
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
            Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(_currentLobby.Id, 30f));
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

            //await Task.WhenAll(InputPlayerData(lobby),RegisterEvents(lobby),Managers.VivoxManager.JoinChannel(lobby.Id));
            await InputPlayerData(lobby);
            await Managers.VivoxManager.JoinChannel(lobby.Id);
            await RegisterEvents(lobby);

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
        _callBackEvent = new LobbyEventCallbacks();
        _callBackEvent.PlayerJoined += PlayerJoinedEvent;
        _callBackEvent.PlayerDataAdded += async (playerdaData) => await PlayerDataAdded(playerdaData);
        _callBackEvent.PlayerLeft += async (list) =>
        {
            await PlayerLeftEvent(list);
            //TODO: �÷��̾ ������ �ش� �ε����� ���־��Ѵ�.
        };
        //_callBackEvent.PlayerLeft += async (list) =>
        //{
        //    await CheckHostAndInjectionHeartBeat();
        //    //TODO: �÷��̾ ������ ȣ��Ʈ�� �����Ÿ�, ��Ʈ��Ʈ�� �־���� �Ѵ�.
        //};


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

    }
    public async Task CheckHostAndInjectionHeartBeat()
    {

    }
    public async void PlayerJoinedEvent(List<LobbyPlayerJoined> joined)
    {
        try
        {
            if (joined == null)
                return;

            LobbyPlayerJoined joinplayer = joined[joined.Count - 1];

            if (joinplayer.Player.Data == null)
                return;
            await Managers.VivoxManager.SendSystemMessageAsync($"{joinplayer.Player.Data["NickName"].Value}���� �����ϼ̽��ϴ�.");
            Debug.Log($"{CurrentPlayerInfo.PlayerNickName}���� �����ϼ̽��ϴ�.");
        }
        catch (Exception ex)
        {
            Debug.Log($"������ �߻��߽��ϴ� : �κ�ID{_currentLobby.Id} �����ڵ�: {ex}");
        }
    }
    private async Task PlayerDataAdded(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> dictionary)
    {
        foreach (int keyIndex in dictionary.Keys)
        {
            Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>> joinPlayerDict = dictionary[keyIndex];
            PlayersJoinLobbyDict[keyIndex] = joinPlayerDict;
            ChangedOrRemovedLobbyValue<PlayerDataObject> playerValue = joinPlayerDict["NickName"];
            if (playerValue.Equals(default))
            {
                Debug.Log("NickName �����Ͱ� �����ϴ�.");
                return;
            }

            Debug.Log($"���� �κ�ID: {_currentLobby.Id} �κ��̸�: {_currentLobby.Name}");
            await Managers.VivoxManager.SendSystemMessageAsync($"{playerValue.Value.Value}���� �����ϼ̽��ϴ�.");
        }
    }

    private async Task PlayerLeftEvent(List<int> leftPlayerIndices)
    {
        Lobby updatedLobby = null;
        try
        {
            await ReFreshRoomList();
            // �κ� ���� �ٽ� �������� (Ȥ�� �̸� ���ÿ� ĳ���� �κ� ������ ���)
            updatedLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
        }
        catch (Exception ex)
        {
            Debug.LogError($"{ex.Message}");
        }
        foreach (int index in leftPlayerIndices)
        {
            PlayersJoinLobbyDict.Remove(index);
            Debug.Log($"{index}��°�� �÷��̾� ������ ����");
        }
        // await Managers.VivoxManager.SendSystemMessageAsync($"{CurrentPlayerInfo.PlayerNickName}���� �����ϼ̽��ϴ�.");
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
        string playerId = AuthenticationService.Instance.PlayerId;
        List<string> lobbyes = await LobbyService.Instance.GetJoinedLobbiesAsync();
        foreach (string lobby in lobbyes)
        {
            await LobbyService.Instance.RemovePlayerAsync(lobby, playerId); //���� �α��εǾ��ִ� �κ񿡼� ���� ������
            Debug.Log($"{lobby}���� �������ϴ�.");
        }//���� ����Ǿ��ִ� �κ� ��������.
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
                    if (player.Data == null)
                    {
                        Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                        return await Utill.RateLimited(() => InitLobbyScene()); // ��õ�
                    }
                    foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                    {
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
        catch (ArgumentException alreadyAddKey) when (alreadyAddKey.Message.Contains("An item with the same key has already been added"))
        {
            Debug.Log($"{alreadyAddKey}�̹� Ű�� ���� �����ص� ��");
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
    private async Task ShowUpdatedLobbyPlayers()
    {
        try
        {
            QueryResponse lobbies = await GetQueryLobbiesAsyncCustom();
            foreach (var lobby in lobbies.Results)
            {
                Player hostPlayer = lobby.Players.FirstOrDefault(player => player.Id == lobby.HostId);

                Debug.Log($"���� �κ��̸�: {lobby.Name} �κ�ID: {lobby.Id} �κ�ȣ��Ʈ: {lobby.HostId} ȣ��Ʈ�г���: {hostPlayer.Data["NickName"].Value}");
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
            await ShowUpdatedLobbyPlayers();
        }
        catch (Exception ex)
        {
            Debug.Log($"����{ex}");
        }
    }


    public async Task ReFreshRoomList()
    {
        if (isRefreshing || UI_Room_Inventory == null) { return; }

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
        Debug.Log($"�濡 �� �κ� ID{lobby.Id}");
    }
}