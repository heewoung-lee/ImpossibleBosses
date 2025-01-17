using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;

public class VivoxManager
{
    public async Task InitializeAsync()
    {

        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        await VivoxService.Instance.InitializeAsync();
    }


    public async Task LoginToVivoxAsync()
    {
        LoginOptions options = new LoginOptions();
        options.DisplayName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
        options.EnableTTS = true;
        await VivoxService.Instance.LoginAsync(options);
    }
}