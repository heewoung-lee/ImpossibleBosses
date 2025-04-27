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

    private Action _vivoxDoneLoginEvent;

    public event Action VivoxDoneLoginEvent
    {
        add
        {
            if (_vivoxDoneLoginEvent != null && _vivoxDoneLoginEvent.GetInvocationList().Contains(value) == true)
                return;

            _vivoxDoneLoginEvent += value;
        }
        remove
        {
            if (_vivoxDoneLoginEvent == null || _vivoxDoneLoginEvent.GetInvocationList().Contains(value) == false)
            {
                Debug.LogWarning($"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                return;
            }
            _vivoxDoneLoginEvent -= value;
        }
    }
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
            Debug.LogError($"InitializeAsync 에러 발생{ex}");
            throw;
        }
    }


    public async Task LoginToVivoxAsync()
    {
        if (VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("로그인이 되어있음 리턴하겠음");
            return;

        }

        try
        {
            _loginOptions = new LoginOptions();
            _loginOptions.DisplayName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
            _loginOptions.EnableTTS = true;
            await VivoxService.Instance.LoginAsync(_loginOptions);
            _checkDoneLoginProcess = true;
            _vivoxDoneLoginEvent?.Invoke();
            Debug.Log("ViVox 로그인완료");
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoginToVivoxAsync 에러 발생{ex}");
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
                Debug.Log($"Vivox 채널{_currentChanel}지워짐");
                await LeaveEchoChannelAsyncCustom(_currentChanel);
            }
            _currentChanel = chanelID;
            await JoinGroupChannelAsyncCustom(_currentChanel, ChatCapability.TextOnly);
        }
        catch (RequestFailedException requestFailExceoption)
        {
            Debug.Log($"오류발생{requestFailExceoption}");
            await Utill.RateLimited(()=>JoinChannel(chanelID));
        }
        catch(ArgumentException alreadyAddKey) when (alreadyAddKey.Message.Contains("An item with the same key has already been added"))
        {
            Debug.Log($"{alreadyAddKey}이미 키가 있음 무시해도 됨");
        }
        catch (Exception ex) 
        {
            Debug.LogError($"JoinChannel 에러 발생{ex}");
            throw;
        }

    }

    public async Task JoinGroupChannelAsyncCustom(string currentChanel,ChatCapability chatCapbillty)
    {
        try
        {
            //await VivoxService.Instance.LeaveAllChannelsAsync();
            await VivoxService.Instance.JoinGroupChannelAsync(currentChanel, chatCapbillty);
        }
        catch (MintException e) when (e.Message.Contains("Request timeout"))
        {
            Debug.LogError($"LeaveEchoChannelAsync 에러 발생{e}");
            await Utill.RateLimited(async () => await VivoxService.Instance.JoinGroupChannelAsync(currentChanel, chatCapbillty));
            throw;
        }
        catch(Exception authorizedException) when (authorizedException.Message.Contains("not authorized"))
        {
            await VivoxService.Instance.LoginAsync(_loginOptions);
        }
        catch (Exception error)
        {
            Debug.LogError($"에러발생{error}");
            throw;
        }
    }



    public async Task LeaveEchoChannelAsyncCustom(string chanelID)
    {
        try
        {
            //await VivoxService.Instance.LeaveAllChannelsAsync();
            if(VivoxService.Instance.ActiveChannels.ContainsKey(chanelID) != default)
            await VivoxService.Instance.LeaveChannelAsync(chanelID);
        }
        catch (MintException e) when (e.Message.Contains("Request timeout"))
        {
            Debug.LogError($"LeaveEchoChannelAsync 에러 발생{e}");
            await Utill.RateLimited(async () => await LeaveEchoChannelAsyncCustom(chanelID));
            throw;
        }
        catch(Exception error)
        {
            Debug.LogError($"에러발생{error}");
            throw;
        }
    }


    public async Task LogoutOfVivoxAsync()
    {
        try
        {
            Debug.Log("vivox 로그아웃");
            await VivoxService.Instance.LogoutAsync();
            _checkDoneLoginProcess = false;
            _currentChanel = null;
            Debug.Log("채널로비" + _currentChanel);
        }
        catch (Exception ex)
        {
            Debug.LogError($"LogoutOfVivoxAsync 에러 발생{ex}");
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
            await VivoxService.Instance.SendChannelTextMessageAsync(_currentChanel, formattedMessage);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SendSystemMessageAsync error:{ex}");
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
            Debug.LogError($"SendMessageAsync 에러 발생{ex}");
            throw;
        }
    }

    public void InitalizeVivoxEvent()
    {
        Managers.SocketEventManager.LogoutVivoxEvent += LogoutOfVivoxAsync;
    }
}