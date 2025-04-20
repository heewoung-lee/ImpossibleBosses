using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;

public class RelayManager
{

    public Action Spawn_RpcCaller_Event;

    private NetworkManager _netWorkManager;
    private string _joinCode;
    private Allocation _allocation;
    private GameObject _nGO_ROOT_UI;
    private GameObject _nGO_ROOT;
    private NGO_RPC_Caller _nGO_RPC_Caller;
    private Define.PlayerClass _choicePlayerCharacter;
    private Dictionary<ulong, Define.PlayerClass> _choicePlayerCharactersDict = new Dictionary<ulong, Define.PlayerClass>();

    public Define.PlayerClass ChoicePlayerCharacter => _choicePlayerCharacter;
    public Dictionary<ulong, Define.PlayerClass> ChoicePlayerCharactersDict => _choicePlayerCharactersDict;
    public Func<Task> DisconnectPlayerAsyncEvent;
    public Action DisconnectPlayerEvent;
    public Action ConnectPlayerEvent;
    public int CurrentUserCount => _netWorkManager.ConnectedClientsList.Count;


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
                Managers.ResourceManager.Instantiate("Prefabs/NGO/NetworkManager");
                //자꾸 어디서 네트워크종료될때 간헐적으로 하이어라키에 이인스턴스 생기는데 이거 의심 해볼것 
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
            if (_nGO_ROOT_UI == null && _netWorkManager.IsHost)
            {
                _nGO_ROOT_UI = SpawnNetworkOBJ("Prefabs/NGO/NGO_ROOT_UI");
            }
            return _nGO_ROOT_UI;
        }
    }

    public GameObject NGO_ROOT
    {
        get
        {
            if (_nGO_ROOT == null && _netWorkManager.IsHost)
            {
                _nGO_ROOT = SpawnNetworkOBJ("Prefabs/NGO/NGO_ROOT");
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
                foreach (NetworkObject netWorkOBJ in NetworkManagerEx.SpawnManager.SpawnedObjects.Values)
                {
                    if (netWorkOBJ.TryGetComponent(out NGO_RPC_Caller rpccaller))
                    {
                        _nGO_RPC_Caller = rpccaller;
                        break;
                    }
                }
            }
            return _nGO_RPC_Caller;
        }
    }

    public string JoinCode { get => _joinCode; }

    public void RegisterSelectedCharacter(ulong clientId, Define.PlayerClass playerClass)
    {
        Managers.RelayManager.NGO_RPC_Caller.SubmitSelectedCharactertoServerRpc(Managers.RelayManager.NetworkManagerEx.LocalClientId, playerClass.ToString());
        _choicePlayerCharacter = playerClass;
    }


    public void RegisterSelectedCharacterinDict(ulong clientId, Define.PlayerClass playerClass)
    {
        _choicePlayerCharactersDict[clientId] = playerClass;
    }
    public GameObject Load_NGO_Prefab<T>(string name = null, string path = null)
    {
        if (name == null)
            name = typeof(T).Name;

        GameObject go = null;
        if (path == null)
        {
            go = Managers.ResourceManager.Instantiate($"Prefabs/NGO/{name}");
        }
        else
        {
            go = Managers.ResourceManager.Instantiate($"{path}");
        }
        go = SpawnNetworkOBJ(go, NGO_ROOT.transform);
        return go;
    }
    public GameObject Load_NGO_Prefab(string path)
    {
        GameObject networkOBJ = SpawnNetworkOBJ(path, NGO_ROOT.transform);
        return networkOBJ;
    }


    public void SetRPCCaller(GameObject ngo)
    {
        _nGO_RPC_Caller = ngo.GetComponent<NGO_RPC_Caller>();
    }

    public void SpawnToRPC_Caller()
    {
        if (_netWorkManager.IsHost == false)
            return;

        if (NGO_RPC_Caller != null)
            return;

        Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/NGO_RPC_Caller", destroyOption: false);
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

    public GameObject SpawnNetworkOBJ(string ngoPath, Transform parent = null, Vector3 position = default, bool destroyOption = true)
    {
        return SpawnNetworkOBJInjectionOnwer(NetworkManagerEx.LocalClientId, ngoPath, position, parent, destroyOption);
    }
    public GameObject SpawnNetworkOBJ(GameObject ngo, Transform parent = null, Vector3 position = default, bool destroyOption = true)
    {
        return SpawnAndInjectionNGO(ngo, NetworkManagerEx.LocalClientId, position, parent, destroyOption);
    }




    public GameObject SpawnNetworkOBJInjectionOnwer(ulong clientId, string ngoPath, Vector3 position = default, Transform parent = null, bool destroyOption = true)
    {
        GameObject instanceObj = Managers.ResourceManager.Instantiate(ngoPath, parent);
        return SpawnAndInjectionNGO(instanceObj, clientId, position, parent, destroyOption);
    }
    public GameObject SpawnNetworkOBJInjectionOnwer(ulong clientId, GameObject ngo, Vector3 position = default, Transform parent = null, bool destroyOption = true)
    {
        return SpawnAndInjectionNGO(ngo, clientId, position, parent, destroyOption);
    }


    private GameObject SpawnAndInjectionNGO(GameObject instanceObj, ulong clientId, Vector3 position, Transform parent = null, bool destroyOption = true)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsListening == true && Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            instanceObj.transform.position = position;
            NetworkObject networkObj = instanceObj.GetOrAddComponent<NetworkObject>();
            if (networkObj.IsSpawned == false)
            {
                //이쪽에서 풀 객체면 스폰이 아닌 문제는 여기구역은 
                networkObj.SpawnWithOwnership(clientId, destroyOption);
            }
            if (parent != null)
            {
                networkObj.transform.SetParent(parent, false);
            }
        }
        return instanceObj;
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
            Debug.Log("소켓에러");
            return false;
        }
        catch (RelayServiceException ex) when (ex.Message.Contains("join code not found"))
        {
            Debug.LogWarning("로비에 릴레이코드가 유효하지 않음 새로 만들어야함");
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
        NetworkManagerEx.Shutdown();
        _joinCode = null;
    }

    private Task WarpperDisConntionRelay()
    {
        ShutDownRelay();
        return Task.CompletedTask;
    }

    public void OnClientDisconnectEvent(ulong disconntedIndex)
    {
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
        NetworkManagerEx.OnClientDisconnectCallback -= OnClientDisconnectEvent;
        NetworkManagerEx.OnClientDisconnectCallback += OnClientDisconnectEvent;
        NetworkManagerEx.OnClientConnectedCallback -= OnClientconnectEvent;
        NetworkManagerEx.OnClientConnectedCallback += OnClientconnectEvent;
    }

    public void SceneLoadInitalizeRelayServer()
    {
        NetworkManagerEx.NetworkConfig.EnableSceneManagement = false;
        Managers.SocketEventManager.DisconnectRelayEvent += WarpperDisConntionRelay;
        Managers.SocketEventManager.DisconnectApiEvent -= WarpperDisConntionRelay;
        Managers.SocketEventManager.DisconnectApiEvent += WarpperDisConntionRelay;
    }

    public void UnSubscribeCallBackEvent()
    {
        NetworkManagerEx.OnClientDisconnectCallback -= OnClientconnectEvent;
        NetworkManagerEx.OnClientConnectedCallback -= OnClientconnectEvent;
    }



    #region 테스트용 함수
    public void SetPlayerClassforMockUnitTest(Define.PlayerClass playerClass)
    {
        _choicePlayerCharacter = playerClass;
    }
    #endregion 
}