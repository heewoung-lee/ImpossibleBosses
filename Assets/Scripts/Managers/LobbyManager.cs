
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
        _nickname = playerNickname; // ��� �ʵ带 �ʱ�ȭ
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
            // Unity Services �ʱ�ȭ
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
        //��Ʈ��Ʈ �κ� ���� ȣ��Ʈ�� ���� �� 
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
            return await RateLimited(() => TryJoinLobbyByName(lobbyName)); // ��õ�
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
            Debug.Log("�κ񿡼� �÷��̾� ������ ����");
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
                Debug.Log($"�κ� �������{_currentLobby.Name}");

            await JoinRoomInitalize();
            Debug.Log($"�κ� ���̵�: {_currentLobby.Id} \n �κ� �̸�{_currentLobby.Name}");
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


            // ��� �۾��� �Ϸ�� ������ ���
            InitDoneEvent?.Invoke();
            _isDoneInitEvent = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"JoinRoomInitalize �� ���� �߻�: {ex.Message}");
            _isDoneInitEvent = false;
            throw; // ���� ȣ��ο� ���ܸ� ����
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
        //�̺�Ʈ ����

    }
    private async Task PlayerLeftEvent(List<int> list)
    {
        try
        {

            Debug.Log("������Ʈ");
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
        //TODO:���� Player������� �ٲ����
        try
        {
            await Managers.VivoxManager.SendSystemMessageAsync($"{CurrentPlayerInfo.PlayerNickName}���� �����ϼ̽��ϴ�.");
            Debug.Log($"{CurrentPlayerInfo.PlayerNickName}���� �����ϼ̽��ϴ�.");
        }
        catch (Exception ex)
        {
            Debug.Log($"������ �߻��߽��ϴ� : �κ�ID{_currentLobby.Id} �����ڵ�: {ex}");
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
            // ��� �κ� �˻�
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in queryResponse.Results)
            {
                foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
                {
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
            return await RateLimited(() => IsAlreadyLogInID(usernickName)); // ��õ�
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
        await Task.Delay(millisecondsDelay); // ���
        return await action.Invoke(); // ���޹��� �۾� ���� �� ��� ��ȯ
    }
    private async Task RateLimited(Func<Task> action, int millisecondsDelay = 1000)
    {
        Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
        await Task.Delay(millisecondsDelay); // ���
        await action.Invoke(); // ���޹��� �۾� ���� �� ��� ��ȯ
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
            // �ڽ��� �÷��̾� ������ ������Ʈ
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, myPlayer.Id, new UpdatePlayerOptions
            {

                Data = updatedData
            });
            Debug.Log($"�κ� �÷��̾������ֱ�{lobby.Id}{myPlayer.Id}");

        }
        catch (ArgumentException alreadyAddKey) when (alreadyAddKey.Message.Contains("An item with the same key has already been added"))
        {
            Debug.Log($"{alreadyAddKey}�̹� Ű�� ���� �����ص� ��");
        }

    }


    private async Task RemovePlayerData()
    {
        if (_currentLobby == null)
            return;

        Unity.Services.Lobbies.Models.Player myPlayer = _currentLobby.Players.Find(player => player.Id == CurrentPlayerInfo.Id);
        if (myPlayer == null)
            return;


        Debug.Log($"{_currentLobby.Id}{myPlayer.Id}�� �߰ߵ�");
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
            Debug.Log($"Failed to remove player from lobby: {ex.Message} �ٽ� �õ���");
            await RateLimited(() => LogoutAndAllLeaveLobby());
        }

        // ����� ���� �α׾ƿ�
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
            return lobbies.Results; // �κ� ��� ��ȯ
        }
        catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.RateLimited)
        {
            return await RateLimited(() => ViewShowAllLobby()); // ��õ�
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
            // �������� ���� �κ��� �ֽ� ���¸� �ٽ� �޾ƿ´�
            Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            Debug.Log($"Lobby ID: {updatedLobby.Id}, Name: {updatedLobby.Name}");

            // �ֽ� �κ� �ִ� �÷��̾� ��� ��ȸ
            foreach (var player in updatedLobby.Players)
            {
                Debug.Log($"Player ID: {player.Id}");

                // ��: Player Data�� "NickName" Ű�� �ִ��� Ȯ��
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

                //Managers.ResourceManager.Instantiate(); TODO: ���⿡ UI ����
                //���⿡ ������ �Է��ϱ�.���� �ο� ��
            }
            Debug.Log($"ȣ��ƮID{_currentLobby.HostId}");
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