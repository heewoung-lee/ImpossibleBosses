using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using Scene;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace Test.TestScripts.UnitTest
{
    public class RoomSceneMockUnitTest : BaseScene
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
        public bool isSoloTest;

        public override Define.Scene CurrentScene => Define.Scene.RoomScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }
        [Inject] private UIManager _uiManager;


        protected override async Task StartInit()
        {
            base.StartInit();
            await JoinChannel();
            _relayManager.SpawnToRPC_Caller();
            UIRoomCharacterSelect uICharacterSelect = _uiManager.GetSceneUIFromResource<UIRoomCharacterSelect>();
            UIRoomChat uiChatting = _uiManager.GetSceneUIFromResource<UIRoomChat>();
        }

        private async Task JoinChannel()
        {
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
                        await _lobbyManager.CreateLobby(LobbyName, 8,null);
                    }
                }
                else
                {
                    await Task.Delay(1000);
                    Lobby lobby = await _lobbyManager.AvailableLobby(LobbyName);
                    if (lobby == null || lobby.Data == null )
                    {
                        await Utill.RateLimited(async () => await JoinChannel(), 1000);
                        return;
                    }
                    string joinCode = lobby.Data["RelayCode"].Value;
                    await _relayManager.JoinGuestRelay(joinCode);
                }
            }
            _lobbyManager.InitializeLobbyEvent();
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
