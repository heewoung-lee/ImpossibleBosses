using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class RelayManager
{

    private NetworkManager _netWorkManager;
    private string _joinCode;
    private Allocation _allocation;
    private GameObject _nGO_ROOT_UI;
    private GameObject _nGO_ROOT;
    private NGO_RPC_Caller _nGO_RPC_Caller;


    public Define.PlayerClass ChoicePlayerCharacter;

    public Func<Task> DisconnectPlayerAsyncEvent;
    public Action DisconnectPlayerEvent;

    public Action ConnectPlayerEvent;

    public NetworkManager NetworkManagerEx
    {
        get
        {
            if (_netWorkManager != null)
                return _netWorkManager;

            if (NetworkManager.Singleton != null)
            {
                _netWorkManager = NetworkManager.Singleton;
            }
            else
            {
                Managers.ResourceManager.InstantiatePrefab("NGO/NetworkManager");
                NetworkManager.Singleton.SetSingleton();
                return NetworkManager.Singleton;
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
                SpawnNetworkOBJ(_nGO_ROOT_UI);
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
                SpawnNetworkOBJ(_nGO_ROOT);
            }
            return _nGO_ROOT;
        }
    }

    public NGO_RPC_Caller NGO_RPC_Caller
    {
        get
        {
            if (_nGO_RPC_Caller == null)
            {
                GameObject ngo_RPC_Caller;
                ngo_RPC_Caller = Managers.ResourceManager.InstantiatePrefab("NGO/NGO_RPC_Caller");
                SpawnNetworkOBJ(ngo_RPC_Caller);
                _nGO_RPC_Caller = ngo_RPC_Caller.GetComponent<NGO_RPC_Caller>();
            }
            return _nGO_RPC_Caller;
        }
    }


    public string JoinCode { get => _joinCode; }

    public GameObject Load_NGO_ROOT_UI_Module(string path)
    {
        GameObject networkOBJ = Managers.ResourceManager.InstantiatePrefab(path);
        SpawnNetworkOBJ(networkOBJ, NGO_ROOT_UI.transform);
        return networkOBJ;
    }

    public async Task<string> StartHostWithRelay(int maxConnections)
    {
        try
        {
            if (NetworkManagerEx.IsHost && _joinCode != null)
                return _joinCode;

            _allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(_allocation, "dtls");
            NetworkManagerEx.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            if (NetworkManagerEx.StartHost())
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


    public NetworkObjectReference GetNetworkObject(GameObject gameobject)
    {
        if (gameobject.TryGetComponent(out NetworkObject ngo))
        {
            return new NetworkObjectReference(ngo);
        }
        Debug.Log("GameObject hasn't a BaseStats");
        return default;
    }
    public GameObject SpawnNetworkOBJ(GameObject obj, Transform parent = null, bool destroyOption = true)
    {
        return SpawnNetworkOBJInjectionOnwer(NetworkManagerEx.LocalClientId, obj, parent, destroyOption);
    }
    public GameObject SpawnNetworkOBJInjectionOnwer(ulong clientId, GameObject obj, Transform parent = null, bool destroyOption = true)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsListening == true && Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            NetworkObject networkObj = obj.GetOrAddComponent<NetworkObject>();
            networkObj.SpawnWithOwnership(clientId, destroyOption);
            if (parent != null)
            {
                networkObj.transform.SetParent(parent, false);
            }
        }
        return obj;
    }
    public void DeSpawn_NetWorkOBJ(ulong networkObjectID)
    {
        NGO_RPC_Caller.DeSpawnByIDServerRpc(networkObjectID);
    }

    public void DeSpawn_NetWorkOBJ(GameObject ngoGameobject)
    {
        NetworkObjectReference despawnNgo = GetNetworkObject(ngoGameobject);
        NGO_RPC_Caller.DeSpawnByReferenceServerRpc(despawnNgo);
    }

    public async Task<bool> JoinGuestRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManagerEx.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            _joinCode = joinCode;
            return !string.IsNullOrEmpty(joinCode) && NetworkManagerEx.StartClient();
        }
        catch (RelayServiceException ex) when (ex.ErrorCode == 404)
        {
            Debug.Log("家南俊矾");
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
        Debug.Log("OnClickentDisconnectEvent 惯积");
        DisconnectPlayerAsyncEvent?.Invoke();
        DisconnectPlayerEvent?.Invoke();
    }
    public void OnClientconnectEvent(ulong disconntedIndex)
    {
        Debug.Log("OnClientconnectEvent 惯积");
        ConnectPlayerEvent?.Invoke();
    }


    public void InitalizeRelayServer()
    {
        NetworkManagerEx.OnClientDisconnectCallback -= OnClientDisconnectEvent;
        NetworkManagerEx.OnClientDisconnectCallback += OnClientDisconnectEvent;
        NetworkManagerEx.OnClientConnectedCallback -= OnClientconnectEvent;
        NetworkManagerEx.OnClientConnectedCallback += OnClientconnectEvent;
    }

    public void SceneLoadInitalizeRelayServer()
    {
        NetworkManagerEx.NetworkConfig.EnableSceneManagement = false;
        Managers.SocketEventManager.OnApplicationQuitEvent += WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent -= WarpperDisConntion;
        Managers.SocketEventManager.DisconnectApiEvent += WarpperDisConntion;
    }

    public void UnSubscribeCallBackEvent()
    {
        NetworkManagerEx.OnClientDisconnectCallback -= OnClientconnectEvent;
        NetworkManagerEx.OnClientConnectedCallback -= OnClientconnectEvent;
    }
}