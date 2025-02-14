using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting;
using Unity.Services.Multiplayer;

public class RelayManager : IManagerInitializable
{

    private NetworkManager _netWorkManager;

    private NetworkManager NetWorkManager
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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetWorkManager.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            if (NetWorkManager.StartHost())
            {
                return joinCode;
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
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
    }

    public void ShutDownRelay()
    {
        NetWorkManager.Shutdown();
    }

    private Task WarpperDisConntion()
    {
        ShutDownRelay();
        return Task.CompletedTask;
    }

    public void OnClickentDisconnectEvent(ulong disconntedIndex)
    {
        Debug.Log($"{disconntedIndex}유저의 연결이 끊어졌습니다");
    }

    public void Init()
    {
        NetWorkManager.NetworkConfig.EnableSceneManagement = false;
        NetWorkManager.OnClientDisconnectCallback -= OnClickentDisconnectEvent;
        NetWorkManager.OnClientDisconnectCallback += OnClickentDisconnectEvent;
        Managers.SocketEventManager.OnApplicationQuitEvent += WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent -= WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent += WarpperDisConntion;
    }
}