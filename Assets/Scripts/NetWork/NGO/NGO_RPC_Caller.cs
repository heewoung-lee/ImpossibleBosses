using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    }


    [Rpc(SendTo.Server)]
    public void DeSpawnByIDServerRpc(ulong networkID, RpcParams rpcParams = default)
    {
        RelayNetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkID, out NetworkObject ngo);
        ngo.Despawn(true);
    }

    [Rpc(SendTo.Server,RequireOwnership =false)]
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
    public void SpawnPrefabNeedToInitalizeRpc(string path,bool isRequestingOwnershipByYou = false,RpcParams rpcParams= default)
    {
        NetworkObject networkObj = SpawnObjectToResources(path, isRequestingOwnershipByYou, rpcParams);
        NotifyPrefabSpawnedClientRpc(networkObj.NetworkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyPrefabSpawnedClientRpc(ulong networkObjectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject obj))
        {
            if(obj.TryGetComponent(out NGO_InitailizeBase ngoInitalize))
            {
                ngoInitalize.SetInitalze(obj);
            }
        }
    }

    private NetworkObject SpawnObjectToResources(string path,bool isRequestingOwnershipByYou = false, RpcParams rpcParams = default)
    {
        GameObject obj = Managers.ResourceManager.InstantiatePrefab(path);
        NetworkObject networkObj;
        if (isRequestingOwnershipByYou)
        {
            networkObj = Managers.RelayManager.SpawnNetworkOBJInjectionOnwer(rpcParams.Receive.SenderClientId, obj).GetComponent<NetworkObject>();
        }
        else
        {
            networkObj = Managers.RelayManager.SpawnNetworkOBJ(obj).GetComponent<NetworkObject>();
        }
        return networkObj;
    }


    //[Rpc(SendTo.Server)]
    //public void SpawnVFXPrefabServerRpc(string path,float duration,bool isFollowing = false, ulong targerObjectID = INVALIDOBJECTID)
    //{
    //    NetworkObject vfxObj = SpawnObjectToResources(path);
    //    Vector3 targetVector = targerObjectID == INVALIDOBJECTID ? Vector3.zero : 



    //    GameObject particleObject = vfxObj.gameObject;
    //    particleObject.SetActive(false);
    //    ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();
    //    particleObject.transform.position = 
    //    particleObject.transform.SetParent(Managers.VFX_Manager.VFX_Root_NGO);
    //    particleObject.SetActive(true);

    //    float maxDurationTime = 0f;

    //    if (_isCheckNGODict.TryGetValue(path, out ParticleInfo info))
    //    {
    //        if (info.isLooping == true)
    //            return particleObject;
    //    }

    //    if (followTarget != null)
    //    {
    //        Managers.ManagersStartCoroutine(FollowingGenerator(followTarget, particleObject));
    //    }
    //    foreach (ParticleSystem particle in particles)
    //    {
    //        particle.Stop();
    //        particle.Clear();
    //        float duration = 0f;
    //        ParticleSystem.MainModule main = particle.main;

    //        duration = settingDuration <= 0 ? main.duration : settingDuration;
    //        main.duration = duration;
    //        if (particle.GetComponent<ParticleLifetimeSync>())//파티클 시스템중 Duration과 시간을 맞춰야 하는 파티클이 있다면 적용
    //        {
    //            main.startLifetime = duration;
    //        }
    //        else if (duration < particle.main.startLifetime.constantMax)//Duration보다 파티클 생존시간이 큰 경우 파티클 생존시간을 넣는다.
    //        {
    //            maxDurationTime = particle.main.startLifetime.constantMax;
    //        }
    //        else if (maxDurationTime < duration + particle.main.startLifetime.constantMax && particle.GetComponent<ParticleLifetimeSync>() == null)
    //        {
    //            maxDurationTime = duration + particle.main.startLifetime.constantMax;
    //        }
    //        particle.Play();
    //    }
    //    Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
    //    ///필요한거
    //    /// 1. 오브젝트 
    //    /// 2. 부모 트랜트폼
    //    /// 3. 경로
    //    /// 4. 파티클 오브젝트 생성위치
    //    /// 5. 몇초동안 생성
    //    /// 6. 따라다닐껀지 
    //}



}
