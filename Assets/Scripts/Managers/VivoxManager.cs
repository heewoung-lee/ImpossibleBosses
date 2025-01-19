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
        try
        {
            InitalizeVivoxEvent();
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        await VivoxService.Instance.InitializeAsync();
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류","오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }


    public async Task LoginToVivoxAsync()
    {
        if (VivoxService.Instance.IsLoggedIn)
            return;

        try
        {
        LoginOptions options = new LoginOptions();
        options.DisplayName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
        options.EnableTTS = true;
        await VivoxService.Instance.LoginAsync(options);
        await JoinChannel(Managers.LobbyManager.CurrentLobby.Id);
        _checkDoneLoginProcess = true;
        VivoxDoneLoginEvent?.Invoke();
        Debug.Log("ViVox 로그인완료");
        }
        catch(Exception ex)
        {
            UI_AlertDialog alert = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert.SetText("오류", "오류가 발생했습니다.");
            Debug.LogError(ex);
        }
    }
    public async Task JoinChannel(string chanelID)
    {
        try
        {
            _currentChanel = chanelID;
            Debug.Log($"현재{_currentChanel}");
            await VivoxService.Instance.JoinGroupChannelAsync(_currentChanel, ChatCapability.TextAndAudio);
        }
        catch( Exception ex)
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
       catch ( Exception ex)
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
            await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, message);
        }
        catch(Exception ex)
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