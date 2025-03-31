using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class NGO_PoolManager
{
    private NetworkObjectPool _ngoPool;

    public void Set_NGO_Pool(NetworkObject ngo)
    {
        _ngoPool = ngo.gameObject.GetComponent<NetworkObjectPool>();
    }
    public void Create_NGO_Pooling_Object()
    {
        Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("NGO/NGO_Polling");
    }


    public GameObject Pop(GameObject prefab,Transform parantTr = null)
    {
        if (_ngoPool.PooledObjects.TryGetValue(prefab, out UnityEngine.Pool.ObjectPool<NetworkObject> objectPool) == false)
        { //등록이 안되어있으면 등록
            _ngoPool.RegisterPrefabInternal(prefab);
        }
        return _ngoPool.GetNetworkObject(prefab, Vector3.zero, Quaternion.identity).gameObject;
    }

    public void Push(GameObject prefab)
    {
        if (prefab.TryGetComponent(out NetworkObject ngo))
        {
            _ngoPool.ReturnNetworkObject(ngo, prefab);
        }
    }
}