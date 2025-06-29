using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using NetWork.NGO;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Util;
using Zenject;
using Object = System.Object;

namespace GameManagers
{
    public class RelayManager
    {
        
        [Inject] IInstantiate _instantiate;
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] private RelayManager _relayManager;

        
        private Action _spawnRpcCallerEvent;
        private NetworkManager _netWorkManager;
        private string _joinCode;
        private Allocation _allocation;
        private GameObject _nGoRootUI;
        private GameObject _nGoRoot;
        private NgoRPCCaller _ngoRPCCaller;
        private Define.PlayerClass _choicePlayerCharacter;
        private Dictionary<ulong, Define.PlayerClass> _choicePlayerCharactersDict = new Dictionary<ulong, Define.PlayerClass>();

        public event Action SpawnRpcCallerEvent
        {
            add
            {
                if (_spawnRpcCallerEvent != null && _spawnRpcCallerEvent.GetInvocationList().Contains(value) == false)
                    return;

                _spawnRpcCallerEvent += value;
            }
            remove
            {
                if(_spawnRpcCallerEvent == null || _spawnRpcCallerEvent.GetInvocationList().Contains(value) == false)
                {
                    Debug.LogWarning($"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                    return;
                }

                _spawnRpcCallerEvent -= value;
            }
        }


        public void Invoke_Spawn_RPCCaller_Event()
        {
            _spawnRpcCallerEvent?.Invoke();
        }


        public Define.PlayerClass ChoicePlayerCharacter => _choicePlayerCharacter;
        public Dictionary<ulong, Define.PlayerClass> ChoicePlayerCharactersDict => _choicePlayerCharactersDict;
        public int CurrentUserCount => _netWorkManager.ConnectedClientsList.Count;
        public NetworkManager NetworkManagerEx
        {
            get
            {
                if (_netWorkManager == null)
                {
                    if (NetworkManager.Singleton != null)
                    {
                        _netWorkManager = NetworkManager.Singleton;
                    }
                    else
                    {
                        GameObject network = _resourcesLoader.Load<GameObject>("Prefabs/NGO/NetworkManager");
                        network.SetActive(false);
                        GameObject networkObj = _instantiate.InstantiateByObject(network);
                        networkObj.SetActive(true);
                        
                        
                        //_netWorkManager = _instantiate.InstantiateByPath("Prefabs/NGO/NetworkManager").GetComponent<NetworkManager>();
                        // NetworkManager network = _resourcesLoader.Load<NetworkManager>("Prefabs/NGO/NetworkManager");
                        // _netWorkManager = UnityEngine.Object.Instantiate(network);
                        // ProjectContext.Instance.Container.Inject(_netWorkManager);
                        //Managers.DontDestroyOnLoad(_netWorkManager.gameObject);
                        //6.28일 수정: 오브젝트가 생성될떄 부모값이 Null인결우 컨테이너를 통해 인젝션을 하면 컨테이너가 부모를 멋대로 넣음. 그래도 순서를 일반 생성 -> 컨테이너 주입으로 변경 
                        _netWorkManager = NetworkManager.Singleton;
                    }
                }  
                return _netWorkManager;
            }
        }

        public GameObject NgoRootUI
        {
            get
            {
                if (_nGoRootUI == null && _netWorkManager.IsHost)
                {
                    _nGoRootUI = SpawnNetworkObj("Prefabs/NGO/NGO_ROOT_UI");
                }
                return _nGoRootUI;
            }
        }

        public GameObject NgoRoot
        {
            get
            {
                if (_nGoRoot == null && _netWorkManager.IsHost)
                {
                    _nGoRoot = SpawnNetworkObj("Prefabs/NGO/NGO_ROOT");
                }
                return _nGoRoot;
            }
        }
        public NgoRPCCaller NgoRPCCaller
        {
            get
            {
                if (_ngoRPCCaller == null && NetworkManagerEx.SpawnManager != null)
                {
                    foreach (NetworkObject netWorkObj in NetworkManagerEx.SpawnManager.SpawnedObjects.Values)
                    {
                        if (netWorkObj.TryGetComponent(out NgoRPCCaller rpccaller))
                        {
                            _ngoRPCCaller = rpccaller;
                            break;
                        }
                    }
                }
                return _ngoRPCCaller;
            }
        }

        public string JoinCode { get => _joinCode; }

        public void RegisterSelectedCharacter(ulong clientId, Define.PlayerClass playerClass)
        {
            _relayManager.NgoRPCCaller.SubmitSelectedCharactertoServerRpc(_relayManager.NetworkManagerEx.LocalClientId, playerClass.ToString());
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
                go = _instantiate.InstantiateByPath($"Prefabs/NGO/{name}");
            }
            else
            {
                go = _instantiate.InstantiateByPath($"{path}");
            }
            go = SpawnNetworkObj(go, NgoRoot.transform);
            return go;
        }
        public GameObject Load_NGO_Prefab(string path)
        {
            GameObject networkObj = SpawnNetworkObj(path, NgoRoot.transform);
            return networkObj;
        }


        public void SetRPCCaller(GameObject ngo)
        {
            _ngoRPCCaller = ngo.GetComponent<NgoRPCCaller>();
        }

        public void SpawnToRPC_Caller()
        {
            if (_netWorkManager.IsHost == false)
                return;

            if (NgoRPCCaller != null)
                return;

            _relayManager.SpawnNetworkObj("Prefabs/NGO/NgoRPCCaller", destroyOption: false);
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
                Debug.Log($"호출 됐나요 릴레이코드: {_joinCode}");
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

        public GameObject SpawnNetworkObj(string ngoPath, Transform parent = null, Vector3 position = default, bool destroyOption = true)
        {
            return SpawnNetworkOBJInjectionOnwer(NetworkManagerEx.LocalClientId, ngoPath, position, parent, destroyOption);
        }
        public GameObject SpawnNetworkObj(GameObject ngo, Transform parent = null, Vector3 position = default, bool destroyOption = true)
        {
            return SpawnAndInjectionNgo(ngo, NetworkManagerEx.LocalClientId, position, parent, destroyOption);
        }

        public GameObject SpawnNetworkOBJInjectionOnwer(ulong clientId, string ngoPath, Vector3 position = default, Transform parent = null, bool destroyOption = true)
        {
            GameObject loadObj = _resourcesLoader.Load<GameObject>(ngoPath);
            loadObj.SetActive(false);
            GameObject instanceObj = _instantiate.InstantiateByObject(loadObj);
            
            return SpawnAndInjectionNgo(instanceObj, clientId, position, parent, destroyOption);
        }
        public GameObject SpawnNetworkOBJInjectionOnwer(ulong clientId, GameObject ngo, Vector3 position = default, Transform parent = null, bool destroyOption = true)
        {
            ngo.SetActive(false);
            return SpawnAndInjectionNgo(ngo, clientId, position, parent, destroyOption);
        }


        private GameObject SpawnAndInjectionNgo(GameObject instanceObj, ulong clientId, Vector3 position, Transform parent = null, bool destroyOption = true)
        {
            if (_relayManager.NetworkManagerEx.IsListening == true && _relayManager.NetworkManagerEx.IsHost)
            {
                instanceObj.transform.position = position;
                NetworkObject networkObj = _instantiate.GetOrAddComponent<NetworkObject>(instanceObj);
                instanceObj.SetActive(true);
                if (networkObj.IsSpawned == false)
                {
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
            NgoRPCCaller.DeSpawnByIDServerRpc(networkObjectID);
        }

        public void DeSpawn_NetWorkOBJ(GameObject ngoGameobject)
        {
            NetworkObjectReference despawnNgo = GetNetworkObject(ngoGameobject);
            NgoRPCCaller.DeSpawnByReferenceServerRpc(despawnNgo);
        }

        public async Task<bool> IsValidRelayJoinCode(string joinCode)
        {
            try
            {
                // 유효한 경우 할당 객체를 받아옴
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                return true;
            }
            catch (RelayServiceException ex) when (ex.Message.Contains("join code not found"))
            {
                Debug.LogWarning($"RelayCode not Found: {joinCode}");
                return false;
            }
            catch (RelayServiceException ex) when (ex.ErrorCode == 404)
            {
                Debug.LogWarning("RelayCode hasnt Available");
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex}");
                return false;
            }
        }

        public async Task<bool> JoinGuestRelay(string joinCode)
        {
            try
            {
                if (NetworkManagerEx.IsClient || NetworkManagerEx.IsServer)
                {
                    Debug.LogWarning("Client or Server is already running.");
                    return false;
                }
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


        public void SceneLoadInitalizeRelayServer()
        {
            //NetworkManagerEx.NetworkConfig.EnableSceneManagement = false;
            //4.21일 주석처리함 과거의 내가 왜 이부분을 넣었는지 이해 안감.
            Managers.SocketEventManager.DisconnectRelayEvent += WarpperDisConntionRelay;
        }

        #region 테스트용 함수
        public void SetPlayerClassforMockUnitTest(Define.PlayerClass playerClass)
        {
            _choicePlayerCharacter = playerClass;
        }
        #endregion 
    }
}