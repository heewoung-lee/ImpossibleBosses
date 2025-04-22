using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
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
    public const ulong INVALIDOBJECTID = ulong.MaxValue;//Ÿ�� ������Ʈ�� �ְ� ���� �������� ���� ���

    [SerializeField]private NetworkVariable<int> _loadedPlayerCount = new NetworkVariable<int>
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
            if(IsHost == false)
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
        Managers.RelayManager.SpawnNetworkOBJInjectionOnwer(clientId, $"Prefabs/Player/{choiceCharacterName}Base", targetPosition, Managers.RelayManager.NGO_ROOT.transform);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.RelayManager.SetRPCCaller(gameObject);
        Managers.RelayManager.Invoke_Spawn_RPCCaller_Event();
        _loadedPlayerCount.OnValueChanged += LoadedPlayerCountValueChanged;
        _isAllPlayerLoaded.OnValueChanged += IsAllPlayerLoadedValueChanged;
    }


    public void IsAllPlayerLoadedValueChanged(bool previosValue,bool newValue)
    {
        SetisAllPlayerLoadedRpc(newValue);
    }
    private void LoadedPlayerCountValueChanged(int previousValue, int newValue)
    {
        Debug.Log($"������{previousValue} ���İ�{newValue}");
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
    public void Spawn_Loot_ItemRpc(IteminfoStruct itemStruct, Vector3 dropPosition, bool destroyOption = true)
    {
        //���⿡�� itemStruct�� IItem���� ��ȯ
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
        //���⿡���� � �������� �����Ҳ��� �������� ���� ������ ��.

        networkLootItem.GetComponent<LootItem>().SetPosition(dropPosition);
        GameObject rootItem = Managers.RelayManager.SpawnNetworkOBJ(networkLootItem, Managers.LootItemManager.ItemRoot);
        NetworkObjectReference rootItemRef = Managers.RelayManager.GetNetworkObject(rootItem);
        SetDropItemInfoRpc(itemStruct, rootItemRef);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetDropItemInfoRpc(IteminfoStruct itemStruct, NetworkObjectReference rootitemRef)
    {
        if (rootitemRef.TryGet(out NetworkObject ngo))
        {
            IItem iteminfo = Managers.ItemDataManager.GetItem(itemStruct.ItemNumber).SetIItemEffect(itemStruct);
            ngo.GetComponent<LootItem>().SetIteminfo(iteminfo);
        }
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
            return SpawnObjectToResources(path, position, parentTr: Managers.NGO_PoolManager.Pool_NGO_Root_Dict[path]);
        }
        return SpawnObjectToResources(path, position, Managers.VFX_Manager.VFX_Root_NGO);
    }


    private NetworkObject SpawnObjectToResources(string path, Vector3 position = default, Transform parentTr = null)
    {
        GameObject obj = Managers.ResourceManager.Instantiate(path);
        obj.transform.position = position;
        NetworkObject networkObj;
        networkObj = Managers.RelayManager.SpawnNetworkOBJ(obj, parentTr).GetComponent<NetworkObject>();
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
            if (paricleNgo.TryGetComponent(out NGO_Skill_Initailize_Base skillInitailze))
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

        if (Managers.BufferManager.GetModifier(effect) is Duration_Buff durationbuff)
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

    [Rpc(SendTo.ClientsAndHost)]
    public void AllClientDisconnetedVivoxAndLobbyRpc()
    {
        _ = DisconnectFromVivoxAndLobby();
        //�� ��ȯ�� ���� �κ�� �񺹽��� ������ ���� ������ �ʿ�
    }
    private async Task DisconnectFromVivoxAndLobby()
    {
        try
        {
            await Managers.LobbyManager.ExitLobbyAsync(await Managers.LobbyManager.GetCurrentLobby(),false);
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
           if(loading.TryGetComponent(out GamePlaySceneLoadingProgress loadingProgress))
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
    public void SubmitSelectedCharactertoServerRpc(ulong clientId,string selectCharacterName)
    {
        Define.PlayerClass selectCharacter = (Define.PlayerClass)Enum.Parse(typeof(Define.PlayerClass), selectCharacterName);
        Managers.RelayManager.RegisterSelectedCharacterinDict(clientId, selectCharacter);
    }
}
