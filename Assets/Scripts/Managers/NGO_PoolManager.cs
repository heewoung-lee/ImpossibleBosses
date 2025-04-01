using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class NGO_PoolManager
{
    private NetworkObjectPool _ngoPool;

    public NetworkObjectPool NgoPool => _ngoPool;

    public void Set_NGO_Pool(NetworkObject ngo)
    {
        _ngoPool = ngo.gameObject.GetComponent<NetworkObjectPool>();
    }
    public void Create_NGO_Pooling_Object()
    {
        Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("NGO/NGO_Polling");
    }


    public GameObject Pop(string prefabPath,Transform parantTr = null)
    {
        if (_ngoPool.PooledObjects.TryGetValue(prefabPath, out UnityEngine.Pool.ObjectPool<NetworkObject> objectPool) == false)
        { //등록이 안되어있으면 등록
            _ngoPool.RegisterPrefabInternal(prefabPath);
        }
        return _ngoPool.GetNetworkObject(prefabPath, Vector3.zero, Quaternion.identity).gameObject;
    }

    public void Push(GameObject prefab)
    {
        if (prefab.TryGetComponent(out NetworkObject ngo))
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                _ngoPool.ReturnNetworkObject(ngo, prefab);
            }
        }
    }

    public void RegisterNGOPoolObjectDict(string path)
    {
        if (Managers.NGO_PoolManager.NgoPool.PooledObjects.ContainsKey(path) == false)
        {
            Managers.NGO_PoolManager.NgoPool.RegisterPrefabInternal(path);
            Debug.Log(Managers.NGO_PoolManager.NgoPool.PooledObjects.ContainsKey(path));
        }
    }


    public bool isNGOPoolObject(GameObject obj,out Poolable poolable)
    {
        return obj.TryGetComponent(out poolable);
    }



    public GameObject SpawnNetObjectFromPool(string path)
    {
        if (NgoPool == null)
        {
            return null;
        }
        GameObject poolNGO = null;
        if (Managers.NGO_PoolManager.NgoPool.PooledObjects.TryGetValue(path, out UnityEngine.Pool.ObjectPool<NetworkObject> objectPool))
        {
            poolNGO = Pop(path);
        }
        return poolNGO;
    }

}