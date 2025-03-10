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
using UnityEditor.PackageManager;
using Unity.Services.Lobbies.Models;
using NUnit.Framework;
using System.Collections.Generic;

public class RelayManager
{

    private NetworkManager _netWorkManager;
    private string _joinCode;
    private Allocation _allocation;
    private GameObject _nGO_ROOT_UI;
    private GameObject _nGO_ROOT;

    public Define.PlayerClass ChoicePlayerCharacter;

    public Func<Task> DisconnectPlayerAsyncEvent;
    public Action DisconnectPlayerEvent;

    public Action ConnectPlayerEvent;


    public NetworkManager NetWorkManager
    {
        get
        {
            if (_netWorkManager is null)
            {
                _netWorkManager = Managers.ResourceManager.InstantiatePrefab("NGO/NetworkManager").GetComponent<NetworkManager>();
            }
            return _netWorkManager;
        }
    }
    public GameObject NGO_ROOT_UI
    {
        get
        {
            if (_nGO_ROOT_UI == null)
            {
                _nGO_ROOT_UI = Managers.ResourceManager.InstantiatePrefab("NGO/NGO_ROOT_UI");
                SpawnNetworkOBJ(_netWorkManager.LocalClientId, _nGO_ROOT_UI, destroyOption: true);
            }
            return _nGO_ROOT_UI;
        }
    }

    public GameObject NGO_ROOT
    {
        get
        {
            if (_nGO_ROOT == null)
            {
                _nGO_ROOT = Managers.ResourceManager.InstantiatePrefab("NGO/NGO_ROOT");
                SpawnNetworkOBJ(_netWorkManager.LocalClientId, _nGO_ROOT, destroyOption: true);
            }
            return _nGO_ROOT;
        }
    }
    public string JoinCode { get => _joinCode; }

    public GameObject Load_NGO_ROOT_UI_Module(string path)
    {
        GameObject networkOBJ = Managers.ResourceManager.InstantiatePrefab(path);
        SpawnNetworkOBJ(_netWorkManager.LocalClientId, networkOBJ, NGO_ROOT_UI.transform, destroyOption: true);
        return networkOBJ;
    }

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

    public GameObject SpawnNetworkOBJ(ulong clientId, GameObject obj, Transform parent = null, bool destroyOption = false)
    {
        if (NetWorkManager.IsListening == true)
        {
            NetworkObject networkObj = obj.GetOrAddComponent<NetworkObject>();
            networkObj.SpawnWithOwnership(clientId);
            networkObj.DestroyWithScene = destroyOption;
            if (parent != null)
            {
                networkObj.transform.SetParent(parent, false);
            }
        }
        return obj;
    }

    public void DeSpawn_NetWorkOBJ(GameObject go)
    {
        if (NetWorkManager.IsHost)
        {
            if (go.TryGetComponent(out NetworkObject ngo))
            {
                ngo.Despawn(true);
            }
        }
        else
        {
            DeSpawn_NetWorkOBJServerRpc(go);
        }
    }

    [Rpc(SendTo.Server)]
    private void DeSpawn_NetWorkOBJServerRpc(GameObject go, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log("이 RPC를 호출한 클라이언트 ID: " + clientId);
        if (go.TryGetComponent(out NetworkObject ngo))
        {
            ngo.Despawn(true);
        }
    }
    [Rpc(SendTo.Server)]
    public void Spawn_Object_ServerRpc(ulong clientId, GameObject obj,Transform parent = null, bool destroyOption = false)
    {
        SpawnNetworkOBJ(clientId,obj,parent,destroyOption);
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
        if (_netWorkManager != null)
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

    public void OnClientDisconnectEvent(ulong disconntedIndex)
    {
        //if (NetWorkManager.LocalClientId != disconntedIndex)
        //    return;
        Debug.Log("OnClickentDisconnectEvent 발생");
        DisconnectPlayerAsyncEvent?.Invoke();
        DisconnectPlayerEvent?.Invoke();
    }
    public void OnClientconnectEvent(ulong disconntedIndex)
    {
        Debug.Log("OnClientconnectEvent 발생");
        ConnectPlayerEvent?.Invoke();
    }


    public void InitalizeRelayServer()
    {
        NetWorkManager.OnClientDisconnectCallback -= OnClientDisconnectEvent;
        NetWorkManager.OnClientDisconnectCallback += OnClientDisconnectEvent;
        NetWorkManager.OnClientConnectedCallback -= OnClientconnectEvent;
        NetWorkManager.OnClientConnectedCallback += OnClientconnectEvent;
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
        NetWorkManager.OnClientDisconnectCallback -= OnClientconnectEvent;
        NetWorkManager.OnClientConnectedCallback -= OnClientconnectEvent;
    }
}