using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buffer;
using Data.DataType.ItemType.Interface;
using GameManagers;
using GameManagers.Interface.BufferManager;
using GameManagers.Interface.GameManagerEx;
using GameManagers.Interface.ItemDataManager;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
using GameManagers.Interface.VivoxManager;
using NetWork.BaseNGO;
using NetWork.NGO.Interface;
using Scene.GamePlayScene;
using Stats;
using UI.Scene.Interface;
using UI.Scene.SceneUI;
using UI.SubItem;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;

namespace NetWork.NGO
{
    public class NgoRPCCaller : NetworkBehaviour
    {
        public class NgoRPCCallerFactory : NgoZenjectFactory<NgoRPCCaller>
        {
            public NgoRPCCallerFactory(DiContainer container, GameObject ngo)
            {
                _container = container;
                _ngo = ngo;
            }
        }
        
        [Inject] private IUISceneManager _uiSceneManager;
        [Inject] private IInstantiate _instantiate;
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] private IItemGetter _itemGetter;
        [Inject] private ILootItemGetter _lootItemGetter;
        [Inject]private IBufferManager _bufferManager;
        [Inject] IPlayerSpawnManager _gameManagerEx;
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private IVivoxSession _vivoxSession;
        [Inject] SceneManagerEx _sceneManagerEx;
        [Inject] private RelayManager _relayManager;
        [Inject] private NgoPoolManager _poolManager;
        
        public const ulong Invalidobjectid = ulong.MaxValue;//타겟 오브젝트가 있고 없고를 가려내기 위한 상수

        private NetworkVariable<int> _loadedPlayerCount = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


        private NetworkVariable<bool> _isAllPlayerLoaded = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public int LoadedPlayerCount
        {
            get { return _loadedPlayerCount.Value; }
            set
            {
                if (IsHost == false)
                    return;

                _loadedPlayerCount.Value = value;
            }
        }

        public bool IsAllPlayerLoaded
        {
            get { return _isAllPlayerLoaded.Value; }
            set
            {
                if (IsHost == false)
                    return;

                _isAllPlayerLoaded.Value = value;
            }
        }
        NetworkManager _networkManager;
        NetworkManager RelayNetworkManager
        {
            get
            {
                if (_networkManager == null)
                {
                    _networkManager = _relayManager.NetworkManagerEx;
                }
                return _networkManager;
            }
        }
        [Rpc(SendTo.Server)]
        public void GetPlayerChoiceCharacterRpc(ulong clientId)
        {
            string choiceCharacterName = _relayManager.ChoicePlayerCharactersDict[clientId].ToString();
            Vector3 targetPosition = new Vector3(1 * clientId, 0, 1);
            _relayManager.SpawnNetworkOBJInjectionOnwer(clientId, $"Prefabs/Player/{choiceCharacterName}Base", targetPosition, _relayManager.NgoRoot.transform,false);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _relayManager.SetRPCCaller(gameObject);
            _relayManager.Invoke_Spawn_RPCCaller_Event();
            _loadedPlayerCount.OnValueChanged += LoadedPlayerCountValueChanged;
            _isAllPlayerLoaded.OnValueChanged += IsAllPlayerLoadedValueChanged;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _loadedPlayerCount.OnValueChanged -= LoadedPlayerCountValueChanged;
            _isAllPlayerLoaded.OnValueChanged -= IsAllPlayerLoadedValueChanged;
        }


        public void IsAllPlayerLoadedValueChanged(bool previosValue, bool newValue)
        {
            SetisAllPlayerLoadedRpc(newValue);
        }
        private void LoadedPlayerCountValueChanged(int previousValue, int newValue)
        {
            //Debug.Log($"이전값{previousValue} 이후값{newValue}");
            LoadedPlayerCountRpc();
        }

        [Rpc(SendTo.Server)]
        public void DeSpawnByIDServerRpc(ulong networkID, RpcParams rpcParams = default)
        {
            RelayNetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkID, out NetworkObject ngo);
            ngo.Despawn(true);
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void DeSpawnByReferenceServerRpc(NetworkObjectReference ngoRef, RpcParams rpcParams = default)
        {
            if (ngoRef.TryGet(out NetworkObject ngo))
            {
                ngo.Despawn(true);
            }
        }


        [Rpc(SendTo.Server)]
        public void Spawn_Loot_ItemRpc(IteminfoStruct itemStruct, Vector3 dropPosition, bool destroyOption = true, NetworkObjectReference addLootItemBehaviour = default)
        {
            //여기에서 itemStruct를 IItem으로 변환
            GameObject networkLootItem = null;
            IItem iteminfo = _itemGetter.GetItemByItemNumber(itemStruct.ItemNumber);
            switch (itemStruct.ItemType)
            {
                case ItemType.Equipment:
                    networkLootItem = _lootItemGetter.GetEquipLootItem(iteminfo);
                    break;
                case ItemType.Consumable:
                    networkLootItem = _lootItemGetter.GetConsumableLootItem(iteminfo);
                    break;
                case ItemType.ETC:
                    break;
            }
            //여기에서는 어떤 아이템을 스폰할껀지 아이템의 형상만 가져올 것.
            networkLootItem.GetComponent<LootItem.LootItem>().SetPosition(dropPosition);
            GameObject rootItem = _relayManager.SpawnNetworkObj(networkLootItem, Managers.LootItemManager.ItemRoot,dropPosition);
            NetworkObjectReference rootItemRef = _relayManager.GetNetworkObject(rootItem);
            SetDropItemInfoRpc(itemStruct, rootItemRef, addLootItemBehaviour);
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void SetDropItemInfoRpc(IteminfoStruct itemStruct, NetworkObjectReference rootitemRef, NetworkObjectReference addLootItemBehaviour)
        {
            NetworkObject rootitemNGO = null;
            if (rootitemRef.TryGet(out NetworkObject rootItemngo) == true)
            {
                rootitemNGO = rootItemngo;
            }
            else
            {
                return;
            }
            if (addLootItemBehaviour.Equals(default(NetworkObjectReference)) == false) // 만약에 루트아이템의 행동동작에 대한 오브젝트가 있다면 설정
            {
                if (addLootItemBehaviour.TryGet(out NetworkObject ngo))
                {
                    ILootItemBehaviour lootItemBehaviour = ngo.GetComponent<ILootItemBehaviour>();
                    if (lootItemBehaviour is MonoBehaviour monoBehaviour)
                    {
                        Type monoBehaviourType = monoBehaviour.GetType();
                        rootitemNGO.gameObject.AddComponent(monoBehaviourType);
                    }
                }
            }
            IItem iteminfo = _itemGetter.GetItemByItemNumber(itemStruct.ItemNumber).SetIItemEffect(itemStruct);
            rootItemngo.GetComponent<LootItem.LootItem>().SetIteminfo(iteminfo);
            rootItemngo.GetComponent<LootItem.LootItem>().SpawnBehaviour();
        }

        [Rpc(SendTo.Server)]
        public void SpawnPrefabNeedToInitializeRpc(string path)
        {
            NetworkObject networkObj = SpawnObjectToResources(path);
            NotifyPrefabSpawnedClientRpc(networkObj.NetworkObjectId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyPrefabSpawnedClientRpc(ulong networkObjectId)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject obj))
            {
                if (obj.TryGetComponent(out NgoInitailizeBase ngoInitialize))
                {
                    ngoInitialize.SetInitalze(obj);
                }
            }
        }
        private NetworkObject SpawnVFXObjectToResources(string path, Vector3 position = default)
        {

            if (_poolManager.PooledObjects.ContainsKey(path))
            {
                //return SpawnObjectToResources(path, position, parentTr: Managers.NGO_PoolManager.Pool_NGO_Root_Dict[path]);
                return SpawnObjectToResources(path, position);
            }
            //4.28일 NGO_CALLER가 부모까지 지정하는건 책임소재에서 문제가 될 수 있어서 이부분은 각자 풀 오브젝트 초기화 부분에서 부모를 지정하도록 함
            return SpawnObjectToResources(path, position, Managers.VFXManager.VFXRootNgo);
        }


        private NetworkObject SpawnObjectToResources(string path, Vector3 position = default, Transform parentTr = null)
        {
            GameObject obj = _instantiate.InstantiateByPath(path);
            obj.transform.position = position;
            NetworkObject networkObj;
            networkObj = _relayManager.SpawnNetworkObj(obj, parentTr, position).GetComponent<NetworkObject>();
            return networkObj;
        }


        [Rpc(SendTo.Server)]
        public void SpawnVFXPrefabServerRpc(string path, float duration, ulong targerObjectID = Invalidobjectid)
        {
            Vector3 pariclePos = Vector3.zero;
            if (_relayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(targerObjectID, out NetworkObject targetNgo))
            {
                pariclePos = targetNgo.transform.position;
            }
            NetworkObject vfxObj = SpawnVFXObjectToResources(path, position: pariclePos);
            SpawnVFXPrefabClientRpc(vfxObj.NetworkObjectId, targetNgo.transform.position, path, duration, targerObjectID);
        }
        [Rpc(SendTo.Server)]
        public void SpawnVFXPrefabServerRpc(string path, float duration, Vector3 spawnPosition = default)
        {
            Vector3 pariclePos = spawnPosition;
            NetworkObject vfxObj = SpawnVFXObjectToResources(path, position: pariclePos);
            SpawnVFXPrefabClientRpc(vfxObj.NetworkObjectId, pariclePos, path, duration);
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnVFXPrefabClientRpc(ulong particleNgoid, Vector3 particleGeneratePos, string path, float duration, ulong targetNGOID = Invalidobjectid)
        {
            Action<GameObject> positionAndBehaviorSetterEvent = null;
            if (_relayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(particleNgoid, out NetworkObject paricleNgo))
            {
                if (paricleNgo.TryGetComponent(out NgoParticleInitailizeBase skillInitailze))
                {
                    skillInitailze.SetInitalze(paricleNgo);
                    if (_relayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(targetNGOID, out NetworkObject targetNgo))
                    {
                        skillInitailze.SetTargetInitalze(targetNgo);
                        positionAndBehaviorSetterEvent += (particleGameObject) => { Managers.ManagersStartCoroutine(Managers.VFXManager.FollowingGenerator(targetNgo.transform, particleGameObject)); };
                    }
                    skillInitailze.StartParticle(path, duration, positionAndBehaviorSetterEvent);
                }
            }
        }

        [Rpc(SendTo.Server)]
        public void Call_InitBuffer_ServerRpc(StatEffect effect, string buffIconImagePath = null, float duration = -1)
        {
            Call_InitBuffer_ClicentRpc(effect, buffIconImagePath, duration);
        }


        [Rpc(SendTo.ClientsAndHost)]
        private void Call_InitBuffer_ClicentRpc(StatEffect effect, string buffIconImagePath = null, float duration = -1)
        {
            PlayerStats playerstats = _gameManagerEx.GetPlayer().GetComponent<PlayerStats>();

            if (_bufferManager.GetModifier(effect) is DurationBuff durationbuff)
            {
                Sprite buffImageIcon = _resourcesLoader.Load<Sprite>(buffIconImagePath);
                durationbuff.SetBuffIconImage(buffImageIcon);
                _bufferManager.InitBuff(playerstats, duration, durationbuff, effect.value);
            }
            else
            {
                _bufferManager.InitBuff(playerstats, duration, effect);
            }
        }
        private async Task DisconnectFromVivoxAndLobby()
        {
            try
            {
                Lobby currentLobby = await _lobbyManager.GetCurrentLobby();

                if (currentLobby == null)
                {
                    return;
                }
                await _lobbyManager.RemoveLobbyAsync(currentLobby);
                await _vivoxSession.LogoutOfVivoxAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Disconneted NetWorkError] Error: {e}");
            }
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void LoadedPlayerCountRpc()
        {
            if (_uiSceneManager.Try_Get_Scene_UI(out UILoading loading))
            {
                if (loading.TryGetComponent(out GamePlaySceneLoadingProgress loadingProgress))
                {
                    loadingProgress.SetLoadedPlayerCount(LoadedPlayerCount);
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SetisAllPlayerLoadedRpc(bool isAllplayerLoaded)
        {
            if (_uiSceneManager.Try_Get_Scene_UI(out UILoading loading))
            {
                if (loading.TryGetComponent(out GamePlaySceneLoadingProgress loadingProgress))
                {
                    loadingProgress.SetisAllPlayerLoaded(isAllplayerLoaded);
                }
            }
        }

        [Rpc(SendTo.Server)]
        public void SubmitSelectedCharactertoServerRpc(ulong clientId, string selectCharacterName)
        {
            Define.PlayerClass selectCharacter = (Define.PlayerClass)Enum.Parse(typeof(Define.PlayerClass), selectCharacterName);
            _relayManager.RegisterSelectedCharacterinDict(clientId, selectCharacter);

        }
        public void SpawnLocalObject(Vector3 pos, string objectPath, SpawnParamBase spawnParamBase)
        {
            FixedList32Bytes<Vector3> list = new FixedList32Bytes<Vector3>();
            list.Add(pos);                          // 한 개만 담기
            SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
        }
        public void SpawnNonNetworkObject(List<Vector3> pos, string objectPath, SpawnParamBase spawnParamBase)
        {
            if (pos == null)
                return;
        
            int posCount = pos.Count;
            switch (posCount)
            {
                case <= 2:
                {
                    FixedList32Bytes<Vector3> list = new FixedList32Bytes<Vector3>();
                    foreach (var p in pos) list.Add(p);
                    SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
                    break;
                }
                case <= 5:
                {
                    FixedList64Bytes<Vector3> list = new FixedList64Bytes<Vector3>();
                    foreach (var p in pos) list.Add(p);
                    SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
                    break;
                }
                case <= 10:
                {
                    FixedList128Bytes<Vector3> list = new FixedList128Bytes<Vector3>();
                    foreach (var p in pos) list.Add(p);
                    SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
                    break;
                }
                case <= 42:
                {
                    FixedList512Bytes<Vector3> list = new FixedList512Bytes<Vector3>();
                    foreach (var p in pos) list.Add(p);
                    SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
                    break;
                }
                case <= 340:
                {
                    FixedList4096Bytes<Vector3> list = new FixedList4096Bytes<Vector3>();
                    foreach (var p in pos) list.Add(p);
                    SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
                    break;
                }
                default:
                    Debug.LogError("Too many positions! Maximum supported is 340.");
                    break;
            }
        }
        [Rpc(SendTo.ClientsAndHost)]
        public void SpwanLocalObjectRpc(ForceNetworkSerializeByMemcpy<FixedList32Bytes<Vector3>> posList, FixedString512Bytes path, SpawnParamBase spawnParamBase)
        {
            ProcessLocalSpawn(posList.Value, path, spawnParamBase);
        }
        [Rpc(SendTo.ClientsAndHost)]
        public void SpwanLocalObjectRpc(ForceNetworkSerializeByMemcpy<FixedList64Bytes<Vector3>> posList, FixedString512Bytes path, SpawnParamBase spawnParamBase)
        {
            ProcessLocalSpawn(posList.Value, path, spawnParamBase);
        }
        [Rpc(SendTo.ClientsAndHost)]
        public void SpwanLocalObjectRpc(ForceNetworkSerializeByMemcpy<FixedList128Bytes<Vector3>> posList, FixedString512Bytes path, SpawnParamBase spawnParamBase)
        {
            ProcessLocalSpawn(posList.Value, path, spawnParamBase);
        }
        [Rpc(SendTo.ClientsAndHost)]
        public void SpwanLocalObjectRpc(ForceNetworkSerializeByMemcpy<FixedList512Bytes<Vector3>> posList, FixedString512Bytes path, SpawnParamBase spawnParamBase)
        {
            ProcessLocalSpawn(posList.Value, path, spawnParamBase);
        }
        [Rpc(SendTo.ClientsAndHost)]
        public void SpwanLocalObjectRpc(ForceNetworkSerializeByMemcpy<FixedList4096Bytes<Vector3>> posList, FixedString512Bytes path, SpawnParamBase spawnParamBase)
        {
            ProcessLocalSpawn(posList.Value, path, spawnParamBase);
        }

        private void ProcessLocalSpawn<TList>(TList posList, FixedString512Bytes path, SpawnParamBase spawnParamBase)
            where TList : struct, INativeList<Vector3>
        {
            string objectPath = path.ConvertToString();
            GameObject spawnGo = _resourcesLoader.Load<GameObject>(objectPath);
            if (spawnGo.TryGetComponent<ISpawnBehavior>(out var spawnBehaviour))
            {
                TList fixedList = posList;
                for (int i = 0; i < fixedList.Length; i++)
                {
                    SpawnParamBase spawnParams = spawnParamBase;
                    spawnParams.ArgPosVector3 = fixedList[i];
                    spawnBehaviour.SpawnObjectToLocal(spawnParams, objectPath);
                }
            }
            else
            {
                Debug.LogError($"ISpawnBehavior not found on prefab: {objectPath}");
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ResetManagersRpc()
        {
            Managers.Clear();
        }

        [Rpc(SendTo.Server)]
        public void OnBeforeSceneUnloadRpc()
        {
            foreach(NetworkObject ngo in _relayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
            {
                if(ngo.TryGetComponent(out ISceneChangeBehaviour behaviour))
                {
                    Debug.Log((behaviour as Component).name + "초기화 처리 됨");
                    behaviour.OnBeforeSceneUnload();
                }
            }
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void OnBeforeSceneUnloadLocalRpc()
        {
            _sceneManagerEx.InvokeOnBeforeSceneUnloadLocalEvent();

            _ = DisconnectFromVivoxAndLobby();//비복스 및 로비 연결해제
        }
    }
}