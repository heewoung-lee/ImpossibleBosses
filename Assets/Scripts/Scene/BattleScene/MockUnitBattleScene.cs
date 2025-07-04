using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using NetWork.NGO.UI;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace Scene.BattleScene
{
    public class MockUnitBattleScene : ISceneSpawnBehaviour
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private NgoPoolManager _poolManager;
        [Inject] private RelayManager _relayManager;

        
        public MockUnitBattleScene(Define.PlayerClass playerClass, UILoading uiLoading, bool isSoloTest)
        {
            _playerClass = playerClass;
            _uiLoadingScene = uiLoading;
            _isSoloTest = isSoloTest;
        }
        public enum PlayersTag
        {
            Player1,
            Player2,
            Player3,
            Player4,
            None
        }
        string _playerType = null;
        GameObject _ngoRoot;
        private const string LobbyName = "TestLobby";
        private UILoading _uiLoadingScene;
        private Define.PlayerClass _playerClass;
        private bool _isSoloTest;

        public ISceneMover Nextscene => new GamePlaySceneMover();

        public void Init()
        {
           JoinChannel().FireAndForgetSafeAsync();
        }
        public void SpawnObj()
        {
            if (_relayManager.NetworkManagerEx.IsListening)
            {
                InitNgoBattleSceneOnHost();
            }
            _relayManager.NetworkManagerEx.OnServerStarted += InitNgoBattleSceneOnHost;
            void InitNgoBattleSceneOnHost()
            {
                if (_relayManager.NetworkManagerEx.IsHost)
                {
                    _relayManager.Load_NGO_Prefab<NgoBattleSceneSpawn>();
                    _poolManager.Create_NGO_Pooling_Object();
                }
            }
        }
        private async Task JoinChannel()
        {
            _relayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClient;
            _relayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClient;
            if (_relayManager.NetworkManagerEx.IsListening == false)
            {
                await SetAuthenticationService();
                if (_playerType == "Player1")
                {
                    if (_isSoloTest == true)//나혼자 테스트 할때
                    {
                        await _relayManager.StartHostWithRelay(8);
                    }
                    else
                    {
                        await _lobbyManager.CreateLobby("TestLobby", 8, null);
                    }
                }
                else
                {

                    await Task.Delay(1000);
                    Lobby lobby = await _lobbyManager.AvailableLobby(LobbyName);
                    if (lobby == null || lobby.Data == null)
                    {
                        await Utill.RateLimited(async () => await JoinChannel(), 1000);
                        return;
                    }
                    string joinCode = lobby.Data["RelayCode"].Value;
                    await _relayManager.JoinGuestRelay(joinCode);
                }
            }
        }
        private void ConnectClient(ulong clientID)
        {
            if (_relayManager.NgoRPCCaller == null)
            {
                _relayManager.SpawnRpcCallerEvent += SpawnPlayer;
            }
            else
            {
                SpawnPlayer();
            }
            void SpawnPlayer()
            {
                if (_relayManager.NetworkManagerEx.LocalClientId != clientID)
                    return;
                Define.PlayerClass playerClass =
                    (int)_playerClass + (int)clientID < Enum.GetValues(typeof(Define.PlayerClass)).Length
                        ? (Define.PlayerClass)((int)_playerClass + (int)clientID) : Define.PlayerClass.Archer;
                _relayManager.RegisterSelectedCharacter(clientID, playerClass);
                _relayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientID);
                LoadBattleScene();
            }
        }

        private void LoadBattleScene()
        {
            _uiLoadingScene.gameObject.SetActive(false);
        }
        private async Task SetAuthenticationService()
        {

            _playerType = GetPlayerTag();
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            string playerID = AuthenticationService.Instance.PlayerId;
            _lobbyManager.SetPlayerLoginInfo(new PlayerIngameLoginInfo(_playerType, playerID));
        }
        public string GetPlayerTag()
        {
            string[] tagValue = CurrentPlayer.ReadOnlyTags();

            PlayersTag currentPlayer = PlayersTag.Player1;
            if (tagValue.Length > 0 && Enum.TryParse(typeof(PlayersTag), tagValue[0], out var parsedEnum))
            {
                currentPlayer = (PlayersTag)parsedEnum;
            }
            return Enum.GetName(typeof(PlayersTag), currentPlayer);
        }
    }
}
