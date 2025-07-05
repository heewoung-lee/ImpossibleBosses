using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using GameManagers.Interface.UIManager;
using NetWork.NGO.UI;
using UI.Scene.SceneUI;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace Scene.GamePlayScene.Legacy
{
    public class MockUnitGamePlayScene : ISceneSpawnBehaviour
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private RelayManager _relayManager;
        [Inject] private IUISceneManager _uisceneManager;
        
        
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
        private Define.PlayerClass _playerClass;
        private bool _isSoloTest;
        private UIStageTimer _uiStageTimer;


        private async Task JoinChannel()
        {
            _relayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClicent;
            _relayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClicent;
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

        private void ConnectClicent(ulong clientID)
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

                _relayManager.RegisterSelectedCharacter(clientID, _playerClass);
                _relayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientID);
                LoadGamePlayScene();
            }
        }

        private void LoadGamePlayScene()
        {
            _uisceneManager.GetOrCreateSceneUI<UILoading>().gameObject.SetActive(false);
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


        public void Init()
        {
            //await JoinChannel(); // 메인 스레드에서 안전하게 실행됨
            _relayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClicent;
            _relayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClicent;
        }
        public void SpawnObj()
        {
            if (_relayManager.NetworkManagerEx.IsListening)
            {
                InitNgoPlaySceneOnHost();
            }
            _relayManager.NetworkManagerEx.OnServerStarted += InitNgoPlaySceneOnHost;
            void InitNgoPlaySceneOnHost()
            {
                if (_relayManager.NetworkManagerEx.IsHost)
                {
                    _relayManager.Load_NGO_Prefab<NgoGamePlaySceneSpawn>();
                }
            }
        }
    }
}
