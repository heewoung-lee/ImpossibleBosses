using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class NGO_RPC_Caller : NetworkBehaviour
{
    public const ulong INVALIDOBJECTID = ulong.MaxValue;//Ÿ�� ������Ʈ�� �ְ� ���� �������� ���� ���


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
    //        if (particle.GetComponent<ParticleLifetimeSync>())//��ƼŬ �ý����� Duration�� �ð��� ����� �ϴ� ��ƼŬ�� �ִٸ� ����
    //        {
    //            main.startLifetime = duration;
    //        }
    //        else if (duration < particle.main.startLifetime.constantMax)//Duration���� ��ƼŬ �����ð��� ū ��� ��ƼŬ �����ð��� �ִ´�.
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
    //    ///�ʿ��Ѱ�
    //    /// 1. ������Ʈ 
    //    /// 2. �θ� Ʈ��Ʈ��
    //    /// 3. ���
    //    /// 4. ��ƼŬ ������Ʈ ������ġ
    //    /// 5. ���ʵ��� ����
    //    /// 6. ����ٴҲ��� 
    //}



}
