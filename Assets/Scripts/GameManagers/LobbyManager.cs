using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scene;
using UI.SubItem;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;

namespace GameManagers
{
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

    public class LobbyManager : IManagerEventInitialize
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
        private const string WaitLobbyName = "WaitLobby";
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
                UniqueEventRegister.AddSingleEvent(ref _lobbyLoading, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _lobbyLoading, value);
            }
        }
        public event Action InitDoneEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _initDoneEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _initDoneEvent, value);
            }
        }
        public event Action HostChageEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _hostChangeEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _hostChangeEvent, value);
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
                // Unity Services 초기화
                isalready = await IsAlreadyLogInNickNameinLobby(_currentPlayerInfo.PlayerNickName);
                _taskChecker[(int)LoadingProcess.CheckAlreadyLogInID] = true;
                if (isalready is true)
                {
                    Debug.Log("이미 접속되어있음");
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
                InitializeVivoxEvent();
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
                            if (player.Data == null && PlayerID == player.Id)//나인데 데이터를 할당 못받았으면,다시 초기화
                            {
                                Debug.LogError($" Player {player.Id} in lobby {lobby.Id} has NULL Data!");
                                return await Utill.RateLimited(() => InitLobbyScene(), 5000); // 재시도
                            }
                            foreach (KeyValuePair<string, PlayerDataObject> data in player.Data)
                            {
                                if (player.Data == null)
                                    continue;

                                if (PlayerID == player.Id) //자기자신은 건너뛰기
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
                    return await Utill.RateLimited(() => IsAlreadyLogInNickNameinLobby(usernickName)); // 재시도
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
            Debug.Log("VivoxLogin켜짐");
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
                Debug.Log($"로비의 호스트 ID:{lobby.HostId} 나의 아이디{PlayerID}");
                StopHeartbeat();
                if (lobby.HostId == PlayerID)
                {
                    Debug.Log("하트비트 이식");
                    _heartBeatCoroutine = Managers.ManagersStartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, interval));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }


        private async Task JoinRelayServer(Lobby lobby, Func<Lobby, Task> checkHostAndGuestEvent)
        {
            await checkHostAndGuestEvent?.Invoke(lobby);
        }
        private async Task RegisteLobbyCallBack(Lobby lobby,
            Func<ILobbyChanges,Task> onLobbyChangeEvent,
            Func<List<LobbyPlayerJoined>,Task> onPlayerJoinedEvent = null,
            Func<List<int>,Task> onPlayerLeftEvent = null)
        {
            LobbyEventCallbacks lobbycallbacks = new LobbyEventCallbacks();
            lobbycallbacks.LobbyChanged += (ilobbyChagnges) =>
            {
                _ = onLobbyChangeEvent.SafeFireAndForgetInvoke(ilobbyChagnges);
            };
            lobbycallbacks.PlayerJoined += (lobbyPlayerJoined) =>
            {
                _ = onPlayerJoinedEvent.SafeFireAndForgetInvoke(lobbyPlayerJoined);
            };
            lobbycallbacks.PlayerLeft += (playerLeftList) =>
            {
                _ = onPlayerLeftEvent.SafeFireAndForgetInvoke(playerLeftList);
            };
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
                Debug.Log(lobby.Name + "로비의 이름");
                await InjectionRelayJoinCodeintoLobby(lobby, joincode);
            }
            catch (LobbyServiceException timeLimmitException) when (timeLimmitException.Message.Contains("Rate limit has been exceeded"))
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
                Debug.Log($"릴레이 코드가 존재하지 않습니다.{exception}");
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
                await ExitLobbyAsync(_currentLobby); //이쪽은 문제 없음

            try
            {
                CreateLobbyOptions waitLobbyoption = new CreateLobbyOptions()
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        {WaitLobbyName,new DataObject(DataObject.VisibilityOptions.Public,"PlayerWaitLobbyRoom") }
                    }
                };
                await TryJoinLobbyByNameOrCreateLobby(WaitLobbyName, 100, waitLobbyoption);
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

        public async Task<Lobby> AvailableLobby(string lobbyname)
        {

            List<Lobby> fillteredLobbyList = null;
            QueryLobbiesOptions lobbyNameFillter = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.Name,
                        op: QueryFilter.OpOptions.EQ, // EQ는 "같은 이름"일 때만
                        value: lobbyname)        // 이 이름과 정확히 일치하는 로비만 검색
                }
            };
            QueryResponse queryResponse = await GetQueryLobbiesAsyncCustom(lobbyNameFillter);

            if (queryResponse is null) //이름으로 못찾았다.
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
                Func<ILobbyChanges,Task> waitLobbyDataChangedEvent = null;
                waitLobbyDataChangedEvent += ilobbychagnes => _ = OnLobbyHostChangeEvent(ilobbychagnes);
                waitLobbyDataChangedEvent += ilobbychagnes => _ = NotifyPlayerCreateRoomEvent(ilobbychagnes);
                await JoinLobbyInitalize(currentLobby, waitLobbyDataChangedEvent);
                await Managers.VivoxManager.SendSystemMessageAsync($"{CurrentPlayerInfo.PlayerNickName}님이 접속하셨습니다");
            }
            catch (LobbyServiceException alreayException) when (alreayException.Message.Contains("player is already a member of the lobby"))
            {
                Debug.Log("플레이어가 이미 접속중입니다. 정보삭제후 재진입을 시도 합니다.");
                Managers.SceneManagerEx.SetCheckTaskChecker(_taskChecker);
                await InitLobbyScene();
            }
            catch (LobbyServiceException TimeLimmitException) when (TimeLimmitException.Message.Contains("Rate limit has been exceeded"))
            {
                await Utill.RateLimited(() => TryJoinLobbyByNameOrCreateLobby(lobbyName, maxPlayers, lobbyOption));
            }

            catch (KeyNotFoundException keynotFoundExceoption) when (keynotFoundExceoption.Message.Contains("The given key 'RelayCode' was not present in the dictionary"))
            {
                Debug.Log("릴레이코드가 없습니다. 다시 찾습니다");
                await Utill.RateLimited(() => TryJoinLobbyByNameOrCreateLobby(lobbyName, maxPlayers, lobbyOption));
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

            async Task OnLobbyHostChangeEvent(ILobbyChanges lobbyChanges)
            {
                try
                {
                    if (lobbyChanges.HostId.Value == PlayerID)
                    {
                        Lobby currentLobby = await GetCurrentLobby();
                        CheckHostAndSendHeartBeat(currentLobby);
                    }
                }
                catch (Exception errer)
                {
                    Debug.Log(errer);
                }
            }
            // 모든 LobbyChanged 이벤트에서 호출
            async Task NotifyPlayerCreateRoomEvent(ILobbyChanges lobbyChanges)
            {
                try
                {
                    // PlayerData 영역에 변경이 없다면 종료
                    if (lobbyChanges.PlayerData.Value == null)
                        return;

                    //PlayerData.Value :  Dictionary<int /*player-index*/, LobbyPlayerChanges>
                    foreach (KeyValuePair<int, LobbyPlayerChanges> changedData in lobbyChanges.PlayerData.Value)
                    {
                        LobbyPlayerChanges playerChanges = changedData.Value;

                        //그 플레이어의 Data 중 실제로 값이 바뀐(Changed) 키 목록
                        var dataChanges = playerChanges.ChangedData;

                        if (!dataChanges.Changed || dataChanges.Value == null)
                            continue;                                   // 데이터 변경이 없으면 패스

                        //LastCreatedRoom 필드가 바뀌었는지 확인
                        if (dataChanges.Value.TryGetValue("LastCreatedRoom", out var roomFieldChange) &&
                            roomFieldChange.Changed)                   // ← key 값이 변경된 경우에만
                        {
                            // roomFieldChange.Value : PlayerDataObject
                            string newRoomId = roomFieldChange.Value.Value;
                            Debug.Log($"다른 플레이어가 새 방을 만들었습니다!  RoomId = {newRoomId}");

                            // 필요하다면 즉시 방 목록 갱신
                            await Managers.LobbyManager.ReFreshRoomList();
                        }
                    }
                }
                catch (Exception error)
                {
                    Debug.Log(error);
                }
            }

        }

        public async Task RemoveLobbyAsync(Lobby lobby)
        {
            if (ReferenceEquals(_currentLobby, lobby))
                _currentLobby = null;

            if (lobby.HostId != CurrentPlayerInfo.Id)
                return;

            StopHeartbeat();//하트비트 제거
            Lobby currentLobby = await GetLobbyAsyncCustom(lobby.Id);//현재의 로비를 가져와야한다.
            await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
        }


        public async Task ExitLobbyAsync(Lobby lobby, bool disconnectRelayOption = true)
        {
            if (lobby == null)
                return;

            try
            {
                StopHeartbeat();//하트비트 제거
                Lobby currentLobby = await GetLobbyAsyncCustom(lobby.Id);//현재의 로비를 가져와야한다.
                bool ischeckUserIsHost = currentLobby.HostId == PlayerID;
                bool ischeckUserAloneInLobby = currentLobby.Players.Count <= 1;
                if (disconnectRelayOption == true)
                {
                    Managers.RelayManager.ShutDownRelay();
                }
                if (ischeckUserAloneInLobby && ischeckUserIsHost)
                {//내가 호스트도 로비에 나만 남았다면 로비삭제
                    await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
                    Debug.Log("로비삭제");
                }
                else
                {
                    //마지막에 남은 사람이 나말고 다른 사람도 있는데, 내가 호스트인경우
                    Debug.Log("로비데이터 내 내 데이터 삭제");
                    await RemovePlayerData(lobby);
                    DeleteRelayCodefromLobby(lobby);
                }
                if (ReferenceEquals(_currentLobby, lobby))
                    _currentLobby = null;
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
                Lobby waitLobby = await GetCurrentLobby();

                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                if (_currentLobby != null && waitLobby != null)
                {
                    Debug.Log($"로비 만들어짐{_currentLobby.Name}");
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




        private async Task OnRoomLobbyChangeHostEventAsync(ILobbyChanges lobbyChanges)
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
                throw;
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
            Func<ILobbyChanges,Task> OnLobbyChangeEvent,
            Func<List<LobbyPlayerJoined>, Task> OnPlayerJoinedEvent = null,
            Func<List<int>, Task> OnPlayerLeftEvent = null)
        {
            _isDoneInitEvent = false;
            try
            {
                await InputPlayerDataIntoLobby(lobby);//로비에 있는 player에 닉네임추가
                await Managers.VivoxManager.JoinChannel(lobby.Id);//비복스 연결
                await RegisteLobbyCallBack(lobby, OnLobbyChangeEvent, OnPlayerJoinedEvent, OnPlayerLeftEvent);
                _initDoneEvent?.Invoke(); //호출이 완료되었을때 이벤트 콜백
                _isDoneInitEvent = true;
            }

            catch (Exception ex)
            {
                Debug.LogError($"JoinRoomInitalize 중 오류 발생: {ex}");
                _isDoneInitEvent = false;
                throw; // 상위 호출부에 예외를 전달
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
                Debug.Log($"{lobby}에서 나갔습니다.");
                StopHeartbeat();
            }
            _lobbyLoading?.Invoke(false);
        }


        private async Task<List<Lobby>> CheckAllLobbyinPlayer()
        {
            //필터 옵션에서 모든 플레이어를 검사하는 필터 옵션은 없으므로 따로 만듦
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

                Debug.Log($"로비ID: {lobby.Id} \t 플레이어ID: {PlayerID} 정보가 입력되었습니다.");
            }
            catch (Exception error)
            {
                Debug.LogError($"에러 발생{error}");
            }
        }


        private async Task RemovePlayerData(Lobby lobby)
        {
            Debug.Log($"로비ID{lobby.Id} \t 플레이어ID{PlayerID} 정보가 제거되었습니다.");
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

        public void InitializeVivoxEvent()
        {
            Managers.VivoxManager.VivoxDoneLoginEvent += SetVivoxTaskCheker;
        }
        public void InitalizeLobbyEvent()
        {
            Managers.SocketEventManager.LogoutAllLeaveLobbyEvent += LogoutAndAllLeaveLobby;
        }

        public async Task ReFreshRoomList()
        {
            if (_currentLobby == null)
                return;


            if (_isRefreshing || Managers.UIManager.Try_Get_Scene_UI(out UI_Room_Inventory room_inventory_ui) == false)
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
                Debug.Log($"에러발생:{e}");
                _isRefreshing = false;
                throw;
            }
            _isRefreshing = false;
        }

        private void CreateRoomInitalize(Lobby lobby)
        {

            if (Managers.UIManager.Try_Get_Scene_UI(out UI_Room_Inventory room_inventory_ui) == false)
                return;

            UIRoomInfoPanel infoPanel = Managers.UIManager.MakeSubItem<UIRoomInfoPanel>(room_inventory_ui.Room_Content);
            infoPanel.SetRoomInfo(lobby);
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
                    Unity.Services.Lobbies.Models.Player hostPlayer = lobby.Players.FirstOrDefault(player => player.Id == lobby.HostId);

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
}