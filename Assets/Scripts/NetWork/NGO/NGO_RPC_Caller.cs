using System;
using System.Collections.Generic;
using System.IO;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

public class NGO_RPC_Caller : NetworkBehaviour
{
    public const ulong INVALIDOBJECTID = ulong.MaxValue;//타겟 오브젝트가 있고 없고를 가려내기 위한 상수


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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.RelayManager.SetRPCCaller(gameObject);
        Managers.NGO_PoolManager.Create_NGO_Pooling_Object();
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
            return SpawnObjectToResources(path, position, Managers.NGO_PoolManager.NGO_Tr);
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

}
