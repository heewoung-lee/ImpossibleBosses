using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace Scene.RoomScene
{
    public class RooMSceneConnectOnlineMulti : IRoomConnectOnline
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private RelayManager _relayManager;


        private string _playerType;
        private const string LobbyName = "TestLobby";
        public async Task RoomSceneConnectOnlineStart()
        {
            PlayerIngameLoginInfo playerinfo = await TestMultiUtil.SetAuthenticationService(TestMultiUtil.GetPlayerTag());
            _lobbyManager.SetPlayerLoginInfo(playerinfo);
            
            if (_playerType == "Player1")
            {
                await _lobbyManager.CreateLobby(LobbyName, 8,null);
            }
            else
            {
                await Task.Delay(1000);
                Lobby lobby = await _lobbyManager.AvailableLobby(LobbyName);
                if (lobby == null || lobby.Data == null )
                {
                    await Utill.RateLimited(async () => await RoomSceneConnectOnlineStart(), 1000);
                    return;
                }
                string joinCode = lobby.Data["RelayCode"].Value;
                await _relayManager.JoinGuestRelay(joinCode);
            }
            _lobbyManager.InitializeLobbyEvent();
            _relayManager.SpawnToRPC_Caller();
        }
    }
}
