using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Vivox;
using UnityEngine;
using static System.Net.WebRequestMethods;

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
            Debug.LogError($"InitializeAsync ���� �߻�{ex}");
            throw;
        }
    }


    public async Task LoginToVivoxAsync()
    {
        if (VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("�α����� �Ǿ����� �����ϰ���");
            return;

        }

        try
        {
            _loginOptions = new LoginOptions();
            _loginOptions.DisplayName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
            _loginOptions.EnableTTS = true;
            await VivoxService.Instance.LoginAsync(_loginOptions);
            Lobby lobby = Managers.LobbyManager.CurrentLobby;
            _checkDoneLoginProcess = true;
            VivoxDoneLoginEvent?.Invoke();
            Debug.Log("ViVox �α��οϷ�");
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoginToVivoxAsync ���� �߻�{ex}");
            throw;
        }
    }
    public async Task JoinChannel(string chanelID)
    {
        try
        {
            if (VivoxService.Instance.IsLoggedIn == false)
            {
                await InitializeAsync();
            }

            if (_currentChanel != null)
            {
                Debug.Log($"ä��{_currentChanel}������");
                await LeaveEchoChannelAsync(_currentChanel);
            }
            _currentChanel = chanelID;
            await VivoxService.Instance.JoinGroupChannelAsync(_currentChanel, ChatCapability.TextOnly);
        }
        catch (MintException mint)
        {
            Debug.Log($"�����߻�{mint}");
            await RateLimited(()=>JoinChannel(chanelID));
        }
        catch(ArgumentException alreadyAddKey) when (alreadyAddKey.Message.Contains("An item with the same key has already been added"))
        {
            Debug.Log($"{alreadyAddKey}�̹� Ű�� ���� �����ص� ��");
        }
        catch (Exception ex) 
        {
            Debug.LogError($"JoinChannel ���� �߻�{ex}");
            throw;
        }

    }
    private async Task<T> RateLimited<T>(Func<Task<T>> action, int millisecondsDelay = 1000)
    {
        Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
        await Task.Delay(millisecondsDelay); // ���
        return await action.Invoke(); // ���޹��� �۾� ���� �� ��� ��ȯ
    }
    private async Task RateLimited(Func<Task> action, int millisecondsDelay = 1000)
    {
        Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
        await Task.Delay(millisecondsDelay); // ���
        await action.Invoke(); // ���޹��� �۾� ���� �� ��� ��ȯ
    }


    public async Task LeaveEchoChannelAsync(string chanelID)
    {
        try
        {
            //await VivoxService.Instance.LeaveAllChannelsAsync();
            await VivoxService.Instance.LeaveChannelAsync(chanelID);
        }
        catch (Exception ex)
        {
            Debug.LogError($"LeaveEchoChannelAsync ���� �߻�{ex}");
            throw;
        }
    }


    public async Task LogoutOfVivoxAsync()
    {
        try
        {
            Debug.Log("vivox �α׾ƿ�");
            await VivoxService.Instance.LogoutAsync();
            _checkDoneLoginProcess = false;
            _currentChanel = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LogoutOfVivoxAsync ���� �߻�{ex}");
            throw;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            if (VivoxService.Instance.IsLoggedIn == false)
                return;

            string sendMessageFormmat = $"[{_loginOptions.DisplayName}] {message}";
            await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, sendMessageFormmat);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SendMessageAsync ���� �߻�{ex}");
            throw;
        }
    }

    public async Task SendSystemMessageAsync(string systemMessage)
    {
        try
        {
            if (VivoxService.Instance.IsLoggedIn == false || VivoxService.Instance.ActiveChannels.Any() == false)
                return;

            string formattedMessage = $"<color=#FFD700>[SYSTEM]</color> {systemMessage}";
            Debug.Log(_currentChanel);
            await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, formattedMessage);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SendSystemMessageAsync ���� �߻�{ex}");
            throw;
        }
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.OnApplicationQuitEvent += LogoutOfVivoxAsync;
        Managers.SocketEventManager.DisconnectApiEvent -= LogoutOfVivoxAsync;
        Managers.SocketEventManager.DisconnectApiEvent += LogoutOfVivoxAsync;
    }
}