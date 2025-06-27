using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using NetWork.NGO.UI;
using Scene.BattleScene;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace Scene.GamePlayScene
{
    public class MockUnitGamePlayScene : ISceneSpawnBehaviour
    {
        [Inject] private LobbyManager _lobbyManager;
        public MockUnitGamePlayScene(Define.PlayerClass playerClass,bool isSoloTest)
        {
            _playerClass = playerClass;
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
        private Define.PlayerClass _playerClass;
        private bool _isSoloTest;
        private UIStageTimer _uiStageTimer;
        [Inject] private UIManager _uiManager;

        
        public ISceneMover Nextscene => new BattleSceneMover();

        private async Task JoinChannel()
        {
            Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClicent;
            Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClicent;
            if (Managers.RelayManager.NetworkManagerEx.IsListening == false)
            {
                await SetAuthenticationService();
                if (_playerType == "Player1")
                {
                    if (_isSoloTest == true)//나혼자 테스트 할때
                    {
                        await Managers.RelayManager.StartHostWithRelay(8);
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
                    await Managers.RelayManager.JoinGuestRelay(joinCode);
                }
            }
        }

        private void ConnectClicent(ulong clientID)
        {
            if (Managers.RelayManager.NgoRPCCaller == null)
            {
                Managers.RelayManager.SpawnRpcCallerEvent += SpawnPlayer;
            }
            else
            {
                SpawnPlayer();
            }
            void SpawnPlayer()
            {
                if (Managers.RelayManager.NetworkManagerEx.LocalClientId != clientID)
                    return;

                Managers.RelayManager.RegisterSelectedCharacter(clientID, _playerClass);
                Managers.RelayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientID);
                LoadGamePlayScene();
            }
        }

        private void LoadGamePlayScene()
        {
            _uiManager.GetOrCreateSceneUI<UI_Loading>().gameObject.SetActive(false);
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


        public async void Init()
        {
            await JoinChannel(); // 메인 스레드에서 안전하게 실행됨
            _uiStageTimer = _uiManager.GetOrCreateSceneUI<UIStageTimer>();
            _uiStageTimer.OnTimerCompleted += Nextscene.MoveScene;
        }
        public void SpawnObj()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsListening)
            {
                InitNgoPlaySceneOnHost();
            }
            Managers.RelayManager.NetworkManagerEx.OnServerStarted += InitNgoPlaySceneOnHost;
            void InitNgoPlaySceneOnHost()
            {
                if (Managers.RelayManager.NetworkManagerEx.IsHost)
                {
                    Managers.RelayManager.Load_NGO_Prefab<NgoGamePlaySceneSpawn>();
                }
            }
        }
    }
}
