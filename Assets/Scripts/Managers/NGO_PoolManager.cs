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
    public void Create_NGO_Pooling_Object()
    {
        Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("NGO/NGO_Polling");
    }
}