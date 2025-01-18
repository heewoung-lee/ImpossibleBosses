using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxManager: IManagerEventInitailize
{
    public Action VivoxDoneLoginEvent;
    private bool _checkDoneLoginProcess = false;
    public bool CheckDoneLoginProcess => _checkDoneLoginProcess;
    string _currentChanel = null;
    public async Task InitializeAsync()
    {
        InitalizeEvent();
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        await VivoxService.Instance.InitializeAsync();
    }


    public async Task LoginToVivoxAsync()
    {
        if (VivoxService.Instance.IsLoggedIn)
            return;

        LoginOptions options = new LoginOptions();
        options.DisplayName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
        options.EnableTTS = true;
        await VivoxService.Instance.LoginAsync(options);
        await JoinChannel(Managers.LobbyManager.CurrentLobby.Id);
        _checkDoneLoginProcess = true;
        VivoxDoneLoginEvent?.Invoke();
        Debug.Log("ViVox 로그인완료");
    }
    public async Task JoinChannel(string chanelID)
    {
        _currentChanel = chanelID;
        Debug.Log($"현재{_currentChanel}");
        await VivoxService.Instance.JoinGroupChannelAsync(_currentChanel, ChatCapability.TextAndAudio);

    }


    public async Task LeaveEchoChannelAsync(string chanelID)
    {
        await VivoxService.Instance.LeaveChannelAsync(chanelID);
    }


    public async Task LogoutOfVivoxAsync()
    {
        Debug.Log("vivox 로그아웃");
        await VivoxService.Instance.LogoutAsync();
    }

    public async Task SendMessageAsync(string message)
    {
       await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, message);
    }

    public void InitalizeEvent()
    {
        Managers.OnApplicationQuitEvent += LogoutOfVivoxAsync;
        Managers.DisconnectApiEvent -= LogoutOfVivoxAsync;
        Managers.DisconnectApiEvent += LogoutOfVivoxAsync;
    }
}