using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NGO_PoolManager
{
    private NetworkObjectPool _ngoPool;

    private Dictionary<string,>

    public NetworkObjectPool NgoPool
    {
        get
        {
            if(_ngoPool == null)
            {
                Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc("NGO/NGO_Polling");
            }
            return _ngoPool;    
        }
    }

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