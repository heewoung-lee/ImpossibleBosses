using System;
using System.Threading.Tasks;
using GameManagers;
using NetWork.NGO.UI;
using Scene;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;

namespace Test.TestScripts.UnitTest
{
    public class BattleSceneMockUnitTest : BaseScene
    {
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
        public Define.PlayerClass PlayerClass;
        public bool isSoloTest;

        public override Define.Scene CurrentScene => Define.Scene.BattleScene;



        protected override async void StartInit()
        {
            base.StartInit();
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
            await JoinChannel();
            //Managers.GameManagerEx.Player.GetComponent<Module_Player_Class>().InitializeSkillsFromManager();
        }
        private async Task JoinChannel()
        {
            Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClicent;
            Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClicent;
            if (Managers.RelayManager.NetworkManagerEx.IsListening == false)
            {
                await SetAuthenticationService();
                if (_playerType == "Player1")
                {
                    if (isSoloTest == true)//나혼자 테스트 할때
                    {
                        await Managers.RelayManager.StartHostWithRelay(8);
                    }
                    else
                    {
                        await Managers.LobbyManager.CreateLobby("TestLobby", 8, null);
                    }
                    if (Managers.RelayManager.NetworkManagerEx == true)
                    {
                        Init_NGO_PlayScene_OnHost();
                    }
                }
                else
                {

                    await Task.Delay(1000);
                    Lobby lobby = await Managers.LobbyManager.AvailableLobby(LobbyName);
                    if (lobby.Data == null)
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
                Define.PlayerClass playerClass =
                    (int)PlayerClass + (int)clientID < Enum.GetValues(typeof(Define.PlayerClass)).Length 
                        ? (Define.PlayerClass)((int)PlayerClass + (int)clientID): Define.PlayerClass.Archer;
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

        private void Init_NGO_PlayScene_OnHost()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                Managers.RelayManager.Load_NGO_Prefab<NgoBattleSceneSpawn>();
                Managers.NgoPoolManager.Create_NGO_Pooling_Object();
            }
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
