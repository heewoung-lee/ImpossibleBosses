using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NGO_PoolInitailize : NGO_InitailizeBase
{

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
    public override NetworkObject ParticleNGO => throw new System.NotImplementedException();

    public override void SetInitalze(NetworkObject particleObj)
    {
        throw new System.NotImplementedException();
    }
}

