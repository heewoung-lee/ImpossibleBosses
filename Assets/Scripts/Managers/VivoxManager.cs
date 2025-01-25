using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Vivox;
using UnityEngine;

public class VivoxManager : IManagerEventInitailize
{
    public Action VivoxDoneLoginEvent;
    private bool _checkDoneLoginProcess = false;
    public bool CheckDoneLoginProcess => _checkDoneLoginProcess;
    LoginOptions _loginOptions;
    string _currentChanel = null;
    public async Task InitializeAsync()
    {
        try
        {
            InitalizeVivoxEvent();
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            await VivoxService.Instance.InitializeAsync();
            await LoginToVivoxAsync();
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }


    public async Task LoginToVivoxAsync()
    {
        if (VivoxService.Instance.IsLoggedIn)
            return;

        try
        {
            _loginOptions = new LoginOptions();
            _loginOptions.DisplayName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
            _loginOptions.EnableTTS = true;
            await VivoxService.Instance.LoginAsync(_loginOptions);
            Lobby lobby = Managers.LobbyManager.CurrentLobby;
            _checkDoneLoginProcess = true;
            VivoxDoneLoginEvent?.Invoke();
            Debug.Log("ViVox 로그인완료");
        }
        catch
        {
            throw;
        }
    }
    public async Task JoinChannel(string chanelID)
    {
        try
        {
            if(VivoxService.Instance.IsLoggedIn == false)
            {
                await InitializeAsync();
            }
            _currentChanel = chanelID;
            Debug.Log($"현재채널ID:{_currentChanel}");
            await VivoxService.Instance.JoinGroupChannelAsync(_currentChanel, ChatCapability.TextAndAudio);
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }

    }


    public async Task LeaveEchoChannelAsync(string chanelID)
    {
        try
        {
            await VivoxService.Instance.LeaveChannelAsync(chanelID);
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }


    public async Task LogoutOfVivoxAsync()
    {
        try
        {
            Debug.Log("vivox 로그아웃");
            await VivoxService.Instance.LogoutAsync();
            _checkDoneLoginProcess = false;
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            string sendMessageFormmat = $"[{_loginOptions.DisplayName}] {message}";
            await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, sendMessageFormmat);
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }

    public async Task SendSystemMessageAsync(string systemMessage)
    {
        try
        {
            string formattedMessage = $"<color=#FFD700>[SYSTEM]</color> {systemMessage}";

            await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, formattedMessage);
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutOfVivoxAsync;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutOfVivoxAsync;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutOfVivoxAsync;
    }
}