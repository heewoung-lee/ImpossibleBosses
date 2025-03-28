using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class NGO_PoolManager
{
    private NetworkObjectPool _ngoPool;

    public NetworkObjectPool NetworkObjectPool => _ngoPool;
    public void Set_NGO_Pool(NetworkObject ngo)
    {
        _ngoPool = ngo.gameObject.GetComponent<NetworkObjectPool>();
    }


    //public Poolable Pop(NetworkObject ngo, Transform parent = null)
    //{
    //    if (_pools.ContainsKey(go.name) == false)
    //        CreatePool(go);

    //    return _pools[go.name].Pop(parent);
    //}



}