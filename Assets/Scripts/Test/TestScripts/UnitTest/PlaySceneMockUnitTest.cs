using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using Scene;
using Scene.GamePlayScene;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace Test.TestScripts.UnitTest
{
    public class PlaySceneMockUnitTest : BaseScene
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private RelayManager _relayManager;

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
        private UI_Loading _uiLoadingScene;
        [Inject] private UIManager _uiManager;

        
        public Define.PlayerClass PlayerClass;
        public bool isSoloTest;

        public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }

        protected override async void StartInit()
        {
            base.StartInit();
            _uiLoadingScene = _uiManager.GetOrCreateSceneUI<UI_Loading>();
            await JoinChannel();
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
                    if (isSoloTest == true)//나혼자 테스트 할때
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
                    if (lobby.Data == null)
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

                _relayManager.RegisterSelectedCharacter(clientID, PlayerClass);
                _relayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientID);
                LoadGamePlayScene();
            }
        }

        private void LoadGamePlayScene()
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

        protected override void AwakeInit()
        {
        }

        public override void Clear()
        {
        }
    }
}