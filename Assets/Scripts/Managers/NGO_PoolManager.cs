using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class NGO_PoolManager : IManagerIResettable
{
    private NetworkObjectPool _ngoPool;
    public Dictionary<string, ObjectPool<NetworkObject>> PooledObjects => _ngoPool.PooledObjects;

    public NetworkObjectPool NGOPool => _ngoPool;

    private Dictionary<string, Transform> _pool_NGO_Root_Dict = new Dictionary<string, Transform>();
    public Dictionary<string, Transform> Pool_NGO_Root_Dict => _pool_NGO_Root_Dict;

    public void Set_NGO_Pool(NetworkObjectPool ngo)
    {
        _ngoPool = ngo;
    }
    public void Create_NGO_Pooling_Object()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false || _ngoPool != null)
            return;

        if (Managers.RelayManager.NGO_RPC_Caller == null)
        {
            Managers.RelayManager.Spawn_RpcCaller_Event += Spawn_Ngo_Polling;
        }
        else
        {
            Spawn_Ngo_Polling();
        }

        void Spawn_Ngo_Polling()
        {
            Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("Prefabs/NGO/NGO_Polling");
        }
    }
    public void SetPool_NGO_ROOT_Dict(string poolNgoPath,Transform RootTr)
    {
        _pool_NGO_Root_Dict.Add(poolNgoPath, RootTr);
    }
    public GameObject Pop(string prefabPath,Transform parantTr = null)
    {
        return _ngoPool.GetNetworkObject(prefabPath, Vector3.zero, Quaternion.identity).gameObject;
    }
    public void Push(NetworkObject ngo)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            ngo.Despawn();
        }
    }

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

    public void NGO_Pool_RegisterPrefab(string path,int capacity = 5)
    {
        _ngoPool.RegisterPrefabInternal(path, capacity);
    }

    public void Clear()
    {
        _pool_NGO_Root_Dict.Clear();
    }

}