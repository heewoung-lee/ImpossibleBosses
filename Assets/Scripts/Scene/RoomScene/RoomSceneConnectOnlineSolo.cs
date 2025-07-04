using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.LoginManager;
using Scene.CommonInstaller;
using Util;
using Zenject;

namespace Scene.RoomScene
{
    internal class RoomSceneConnectOnlineSolo: ISceneConnectOnline
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private RelayManager _relayManager;


        private string _playerType;
        private const string LobbyName = "TestLobby";
        public async Task SceneConnectOnlineStart()
        {
            PlayerIngameLoginInfo playerinfo = await TestMultiUtil.SetAuthenticationService(TestMultiUtil.GetPlayerTag());
            _lobbyManager.SetPlayerLoginInfo(playerinfo);
            await _relayManager.StartHostWithRelay(8);
            _lobbyManager.InitializeLobbyEvent();
            _relayManager.SpawnToRPC_Caller();
        }
    }
}
