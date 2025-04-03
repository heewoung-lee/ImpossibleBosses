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
    //public Dictionary<string, ObjectPool<NetworkObject>> PooledObjects => _ngoPool.PooledObjects;

    public Transform NGO_Tr => _ngoPool.transform;

    public void Set_NGO_Pool(NetworkObjectPool networkOBJPool)
    {
        _ngoPool = networkOBJPool;
    }
    public void Create_NGO_Pooling_Object()
    {
       // Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("Prefabs/NGO/NGO_Polling");
    }

    public GameObject Pop(string prefabPath, Transform parantTr = null)
    {
        GameObject go =  Managers.ResourceManager.Load<GameObject>(prefabPath);
        return _ngoPool.GetNetworkObject(go, Vector3.zero, Quaternion.identity).gameObject;
    }
    //public void RegisterPoolingPrefab(string poolingPath, int capacity)
    //{
    //    _ngoPool.RegisterPrefabInternal(poolingPath, capacity);
    //}
    //private NetworkObject _ngo;
    //public override NetworkObject ParticleNGO => _ngo;

    //public override void SetInitalze(NetworkObject obj)
    //{
    //    _ngo = obj;
    //    Managers.NGO_PoolManager.Set_NGO_Pool(obj);
    //    foreach ((string, int) poolingPath in AutoRegisterFromFolder())
    //    {
    //        Managers.NGO_PoolManager.RegisterPoolingPrefab(poolingPath.Item1, poolingPath.Item2);
    //    }
    //}

    public List<(string, int)> AutoRegisterFromFolder()
    {
        GameObject[] poolableNGOList = Managers.ResourceManager.LoadAll<GameObject>("Prefabs");
        List<(string, int)> poolingOBJ_Path = new List<(string, int)>();
        foreach (GameObject go in poolableNGOList)
        {
            if (go.TryGetComponent(out Poolable poolable) && go.TryGetComponent(out NGO_PoolingInitalize_Base poolingOBJ))
            {
                poolingOBJ_Path.Add((poolingOBJ.PoolingNGO_PATH, poolingOBJ.PoolingCapacity));
            }
        }
        return poolingOBJ_Path;
    }
    public void Push(NetworkObject ngo)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            ngo.Despawn();
        }
    }

}