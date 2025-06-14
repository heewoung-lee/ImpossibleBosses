using System;
using System.Threading.Tasks;
using GameManagers;
using NetWork.NGO.UI;
using Scene.GamePlayScene;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;

namespace Scene.BattleScene
{
    public class MockUnitBattleScene : ISceneSpawnBehaviour
    {
        public MockUnitBattleScene(Define.PlayerClass playerClass, UI_Loading uiLoading, bool isSoloTest)
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
        private UI_Loading _uiLoadingScene;
        private Define.PlayerClass _playerClass;
        private bool _isSoloTest;

        public ISceneMover Nextscene => new GamePlaySceneMover();

        public void Init()
        {
           JoinChannel().FireAndForgetSafeAsync();
        }
        public void SpawnObj()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsListening)
            {
                InitNgoBattleSceneOnHost();
            }
            Managers.RelayManager.NetworkManagerEx.OnServerStarted += InitNgoBattleSceneOnHost;
            void InitNgoBattleSceneOnHost()
            {
                if (Managers.RelayManager.NetworkManagerEx.IsHost)
                {
                    Managers.RelayManager.Load_NGO_Prefab<NgoBattleSceneSpawn>();
                    Managers.NgoPoolManager.Create_NGO_Pooling_Object();
                }
            }
        }
        private async Task JoinChannel()
        {
            Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClient;
            Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClient;
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
                        await Managers.LobbyManager.CreateLobby("TestLobby", 8, null);
                    }
                }
                else
                {

                    await Task.Delay(1000);
                    Lobby lobby = await Managers.LobbyManager.AvailableLobby(LobbyName);
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
        private void ConnectClient(ulong clientID)
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
                Define.PlayerClass playerClass =
                    (int)_playerClass + (int)clientID < Enum.GetValues(typeof(Define.PlayerClass)).Length
                        ? (Define.PlayerClass)((int)_playerClass + (int)clientID) : Define.PlayerClass.Archer;
                Managers.RelayManager.RegisterSelectedCharacter(clientID, playerClass);
                Managers.RelayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientID);
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
            Managers.LobbyManager.SetPlayerLoginInfo(new PlayerIngameLoginInfo(_playerType, playerID));
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
