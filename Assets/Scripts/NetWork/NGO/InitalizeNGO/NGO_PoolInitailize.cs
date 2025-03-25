using Unity.Netcode;
using UnityEngine;

public class NGO_PoolInitailize : NGO_InitailizeBase
{

    private NetworkObject _ngo;
    public override NetworkObject SpawnNgo => _ngo;

    public override void SetInitalze(NetworkObject obj)
    {
        _ngo = obj;
        Managers.NGO_PoolManager.Set_NGO_Pool(obj);
    }
}

