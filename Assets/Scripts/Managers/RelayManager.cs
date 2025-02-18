using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

public class RelayManager
{

    private NetworkManager _netWorkManager;
    private string _joinCode;
    
    private Allocation _allocation;
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

    public string JoinCode { get => _joinCode;}

    public async Task<string> StartHostWithRelay(int maxConnections)
    {
        try
        {
            if (NetWorkManager.IsHost && _joinCode != null)
                return _joinCode;

            _allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(_allocation, "dtls");
            NetWorkManager.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
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
            _joinCode = joinCode;
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
        if(_netWorkManager != null)
        {
            NetworkManager.Singleton.Shutdown();
            _joinCode = null;
        }
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
        NetWorkManager.OnClientDisconnectCallback -= OnClickentDisconnectEvent;
        NetWorkManager.OnClientDisconnectCallback += OnClickentDisconnectEvent;
        Managers.LobbyManager.HostChangedEvent -= JoinGuestRelay;
        Managers.LobbyManager.HostChangedEvent += JoinGuestRelay;
    }

    public void SceneLoadInitalizeRelayServer()
    {
        NetWorkManager.NetworkConfig.EnableSceneManagement = false;
        Managers.SocketEventManager.OnApplicationQuitEvent += WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent -= WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent += WarpperDisConntion;
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