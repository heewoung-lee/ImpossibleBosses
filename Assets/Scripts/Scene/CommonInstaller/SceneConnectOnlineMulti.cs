using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using Unity.Services.Lobbies.Models;
using Util;
using Zenject;

namespace Scene.CommonInstaller
{
    public class SceneConnectOnlineMulti : ISceneConnectOnline
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private RelayManager _relayManager;


        private string _playerType;
        public async Task SceneConnectOnlineStart()
        {
            PlayerIngameLoginInfo playerinfo = await TestMultiUtil.SetAuthenticationService(TestMultiUtil.GetPlayerTag());
            _lobbyManager.SetPlayerLoginInfo(playerinfo);
            _playerType = Util.TestMultiUtil.GetPlayerTag();
            if (_playerType == "Player1")
            {
                await _lobbyManager.CreateLobby(TestMultiUtil.LobbyName, 8,null);
            }
            else
            {
                await Task.Delay(1000);
                Lobby lobby = await _lobbyManager.AvailableLobby(TestMultiUtil.LobbyName);
                if (lobby == null || lobby.Data == null )
                {
                    await Utill.RateLimited(async () => await SceneConnectOnlineStart(), 1000);
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
