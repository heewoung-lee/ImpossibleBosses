using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class RelayManager
{

    private NetworkManager _netWorkManager;
    private string _joinCode;
    public Func<Task> DisconnectPlayerEvent;
    public NetworkManager NetWorkManager
    {
        get
        {
            if( _netWorkManager is null)
            {
                _netWorkManager = Managers.ResourceManager.InstantiatePrefab("Network/NetworkManager").GetComponent<NetworkManager>();
            }
            return _netWorkManager;
        }
    }
    public async Task<string> StartHostWithRelay(int maxConnections)
    {
        try
        {
            if (NetWorkManager.IsHost && _joinCode != null)
                return _joinCode;

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetWorkManager.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            if (NetWorkManager.StartHost())
            {
                return _joinCode;
            }
            return null;

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
    public async Task<bool> JoinGuestRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetWorkManager.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            return !string.IsNullOrEmpty(joinCode) && NetWorkManager.StartClient();
        }
        catch (RelayServiceException ex) when (ex.ErrorCode == 404)
        {
            Debug.Log("소켓에러");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
    }

    public void ShutDownRelay()
    {
        NetworkManager.Singleton.Shutdown();
        NetWorkManager.Shutdown();
        _joinCode = null;
    }

    private Task WarpperDisConntion()
    {
        ShutDownRelay();
        return Task.CompletedTask;
    }

    public void OnClickentDisconnectEvent(ulong disconntedIndex)
    {
        //if (NetWorkManager.LocalClientId != disconntedIndex)
        //    return;


        DisconnectPlayerEvent?.Invoke();
    }
    public void InitalizeRelayServer()
    {
        NetWorkManager.NetworkConfig.EnableSceneManagement = false;
        NetWorkManager.OnClientDisconnectCallback -= OnClickentDisconnectEvent;
        NetWorkManager.OnClientDisconnectCallback += OnClickentDisconnectEvent;
        Managers.SocketEventManager.OnApplicationQuitEvent += WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent -= WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent += WarpperDisConntion;
        Managers.LobbyManager.HostChangedEvent -= JoinGuestRelay;
        Managers.LobbyManager.HostChangedEvent += JoinGuestRelay;
    }

    public void UnSubscribeCallBackEvent()
    {
        NetWorkManager.OnClientDisconnectCallback -= OnClickentDisconnectEvent;
    }
    public void ShowRelayPlayer()
    {
        foreach (NetworkClient player in NetWorkManager.ConnectedClientsList)
        {
            Debug.Log($"현재 접속되어있는 클라이언트 목록{player.ClientId}");
        }
    }
}