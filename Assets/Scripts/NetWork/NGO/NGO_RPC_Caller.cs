using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Buffer;
using Data.DataType.ItemType.Interface;
using Unity.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;
using Scene = UnityEngine.SceneManagement.Scene;

public class NGO_RPC_Caller : NetworkBehaviour
{
    public const ulong INVALIDOBJECTID = ulong.MaxValue;//타겟 오브젝트가 있고 없고를 가려내기 위한 상수

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
                _networkManager = Managers.RelayManager.NetworkManagerEx;
            }
            return _networkManager;
        }
    }
    [Rpc(SendTo.Server)]
    public void GetPlayerChoiceCharacterRpc(ulong clientId)
    {
        string choiceCharacterName = Managers.RelayManager.ChoicePlayerCharactersDict[clientId].ToString();
        Vector3 targetPosition = new Vector3(1 * clientId, 0, 1);
        Managers.RelayManager.SpawnNetworkOBJInjectionOnwer(clientId, $"Prefabs/Player/{choiceCharacterName}Base", targetPosition, Managers.RelayManager.NGO_ROOT.transform,false);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.RelayManager.SetRPCCaller(gameObject);
        Managers.RelayManager.Invoke_Spawn_RPCCaller_Event();
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
        IItem iteminfo = Managers.ItemDataManager.GetItem(itemStruct.ItemNumber);
        switch (itemStruct.Item_Type)
        {
            case ItemType.Equipment:
                networkLootItem = Managers.ItemDataManager.GetEquipLootItem(iteminfo);
                break;
            case ItemType.Consumable:
                networkLootItem = Managers.ItemDataManager.GetConsumableLootItem(iteminfo);
                break;
            case ItemType.ETC:
                break;
        }
        //여기에서는 어떤 아이템을 스폰할껀지 아이템의 형상만 가져올 것.
        networkLootItem.GetComponent<LootItem>().SetPosition(dropPosition);
        GameObject rootItem = Managers.RelayManager.SpawnNetworkOBJ(networkLootItem, Managers.LootItemManager.ItemRoot,dropPosition);
        NetworkObjectReference rootItemRef = Managers.RelayManager.GetNetworkObject(rootItem);
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
        IItem iteminfo = Managers.ItemDataManager.GetItem(itemStruct.ItemNumber).SetIItemEffect(itemStruct);
        rootItemngo.GetComponent<LootItem>().SetIteminfo(iteminfo);
        rootItemngo.GetComponent<LootItem>().SpawnBahaviour();
    }

    [Rpc(SendTo.Server)]
    public void SpawnPrefabNeedToInitalizeRpc(string path)
    {
        NetworkObject networkObj = SpawnObjectToResources(path);
        NotifyPrefabSpawnedClientRpc(networkObj.NetworkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyPrefabSpawnedClientRpc(ulong networkObjectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject obj))
        {
            if (obj.TryGetComponent(out NGO_InitailizeBase ngoInitalize))
            {
                ngoInitalize.SetInitalze(obj);
            }
        }
    }
    private NetworkObject SpawnVFXObjectToResources(string path, Vector3 position = default)
    {

        if (Managers.NGO_PoolManager.PooledObjects.ContainsKey(path))
        {
            //return SpawnObjectToResources(path, position, parentTr: Managers.NGO_PoolManager.Pool_NGO_Root_Dict[path]);
            return SpawnObjectToResources(path, position);
        }
        //4.28일 NGO_CALLER가 부모까지 지정하는건 책임소재에서 문제가 될 수 있어서 이부분은 각자 풀 오브젝트 초기화 부분에서 부모를 지정하도록 함
        return SpawnObjectToResources(path, position, Managers.VFX_Manager.VFX_Root_NGO);
    }


    private NetworkObject SpawnObjectToResources(string path, Vector3 position = default, Transform parentTr = null)
    {
        GameObject obj = Managers.ResourceManager.Instantiate(path);
        obj.transform.position = position;
        NetworkObject networkObj;
        networkObj = Managers.RelayManager.SpawnNetworkOBJ(obj, parentTr, position).GetComponent<NetworkObject>();
        return networkObj;
    }


    [Rpc(SendTo.Server)]
    public void SpawnVFXPrefabServerRpc(string path, float duration, ulong targerObjectID = INVALIDOBJECTID)
    {
        Vector3 pariclePos = Vector3.zero;
        if (Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(targerObjectID, out NetworkObject targetNgo))
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
    public void SpawnVFXPrefabClientRpc(ulong particleNGOID, Vector3 particleGeneratePos, string path, float duration, ulong targetNGOID = INVALIDOBJECTID)
    {
        Action<GameObject> positionAndBehaviorSetterEvent = null;
        if (Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(particleNGOID, out NetworkObject paricleNgo))
        {
            if (paricleNgo.TryGetComponent(out NGO_Particle_Initailize_Base skillInitailze))
            {
                skillInitailze.SetInitalze(paricleNgo);
                if (Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(targetNGOID, out NetworkObject targetNgo))
                {
                    skillInitailze.SetTargetInitalze(targetNgo);
                    positionAndBehaviorSetterEvent += (particleGameObject) => { Managers.ManagersStartCoroutine(Managers.VFX_Manager.FollowingGenerator(targetNgo.transform, particleGameObject)); };
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
        PlayerStats playerstats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();

        if (Managers.BufferManager.GetModifier(effect) is DurationBuff durationbuff)
        {
            Sprite buffImageIcon = Managers.ResourceManager.Load<Sprite>(buffIconImagePath);
            durationbuff.SetBuffIconImage(buffImageIcon);
            Managers.BufferManager.InitBuff(playerstats, duration, durationbuff, effect.value);
        }
        else
        {
            Managers.BufferManager.InitBuff(playerstats, duration, effect);
        }
    }
    private async Task DisconnectFromVivoxAndLobby()
    {
        try
        {
            Lobby currentLobby = await Managers.LobbyManager.GetCurrentLobby();

            if (currentLobby == null)
            {
                return;
            }

            if (currentLobby != null)
            {
                await Managers.LobbyManager.RemoveLobbyAsync(currentLobby);
            }
            await Managers.VivoxManager.LogoutOfVivoxAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Disconneted NetWorkError] Error: {e}");
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void LoadedPlayerCountRpc()
    {
        if (Managers.UI_Manager.Try_Get_Scene_UI(out UI_Loading loading))
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
        if (Managers.UI_Manager.Try_Get_Scene_UI(out UI_Loading loading))
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
        Managers.RelayManager.RegisterSelectedCharacterinDict(clientId, selectCharacter);

    }
    public void SpawnLocalObject(Vector3 pos, string objectPath, SpawnParamBase spawnParamBase)
    {
        FixedList32Bytes<Vector3> list = new FixedList32Bytes<Vector3>();
        list.Add(pos);                          // 한 개만 담기
        SpwanLocalObjectRpc(list, new FixedString512Bytes(objectPath), spawnParamBase);
    }
    public void SpawnNonNetworkObject(List<Vector3> pos, string objectPath, SpawnParamBase spawnParamBase)
    {
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
        GameObject spawnGo = Managers.ResourceManager.Load<GameObject>(objectPath);
        if (spawnGo.TryGetComponent<ISpawnBehavior>(out var spawnBehaviour))
        {
            TList fixedList = posList;
            for (int i = 0; i < fixedList.Length; i++)
            {
                SpawnParamBase spawnParams = spawnParamBase;
                spawnParams.argPosVector3 = fixedList[i];
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
        foreach(NetworkObject ngo in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
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
        Managers.SceneManagerEx.InvokeOnBeforeSceneUnloadLocalEvent();

        _ = DisconnectFromVivoxAndLobby();//비복스 및 로비 연결해제
    }
}