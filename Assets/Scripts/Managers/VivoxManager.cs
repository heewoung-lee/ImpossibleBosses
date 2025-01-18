using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxManager
{
    public async Task InitializeAsync()
    {
        Managers.OnApplicationQuitEvent += LogoutOfVivoxAsync;
        if (UnityServices.State != ServicesInitializationState.Initialized)
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
    public async Task JoinEchoChannel()
    {
        string channelToJoin = "Lobby";
        await VivoxService.Instance.JoinEchoChannelAsync(channelToJoin, ChatCapability.TextAndAudio);
    }


    public async Task LeaveEchoChannelAsync()
    {
        string channelToLeave = "Lobby";
        await VivoxService.Instance.LeaveChannelAsync(channelToLeave);
    }


    public async Task LogoutOfVivoxAsync()
    {
        Debug.Log("vivox ·Î±×¾Æ¿ô");
        await VivoxService.Instance.LogoutAsync();
    }

    
}