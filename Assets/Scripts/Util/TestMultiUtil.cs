using System;
using System.Threading.Tasks;
using GameManagers.Interface.LoginManager;
using Scene.RoomScene;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace Util
{
    public static class TestMultiUtil
    {
        public static async Task<PlayerIngameLoginInfo> SetAuthenticationService(string playerTag)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            string playerID = AuthenticationService.Instance.PlayerId;
            
            return new PlayerIngameLoginInfo(playerTag, playerID);
        }


        public static string GetPlayerTag()
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
