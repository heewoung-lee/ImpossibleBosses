using Unity.Netcode;
using UnityEngine;

public class NGO_PoolInitailize : NGO_InitailizeBase
{
    public override void SetInitalze(NetworkObject obj)
    {
        Managers.NGO_PoolManager.Set_NGO_Pool(obj);
    }
}
