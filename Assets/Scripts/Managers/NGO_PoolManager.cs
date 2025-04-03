using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class NGO_PoolManager
{
    private NetworkObjectPool _ngoPool;
    public Dictionary<string, ObjectPool<NetworkObject>> PooledObjects => _ngoPool.PooledObjects;

    public Transform NGO_Tr => _ngoPool.transform;

    public void Set_NGO_Pool(NetworkObject ngo)
    {
        _ngoPool = ngo.gameObject.GetComponent<NetworkObjectPool>();
    }
    public void Create_NGO_Pooling_Object()
    {
        Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("Prefabs/NGO/NGO_Polling");
    }

    public GameObject Pop(string prefabPath,Transform parantTr = null)
    {
        return _ngoPool.GetNetworkObject(prefabPath, Vector3.zero, Quaternion.identity).gameObject;
    }
    public void RegisterPoolingPrefab(string poolingPath,int capacity)
    {
        _ngoPool.RegisterPrefabInternal(poolingPath, capacity);
    }

    public void Push(NetworkObject ngo)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            ngo.Despawn();
        }
    }

}