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

    //public List<(string,int)> AutoRegisterFromFolder()
    //{
    //    GameObject[] poolableNGOList = Managers.ResourceManager.LoadAll<GameObject>("Prefabs");
    //    List<(string,int)> poolingOBJ_Path = new List<(string, int)>();
    //    foreach (GameObject go in poolableNGOList)
    //    {
    //        if (go.TryGetComponent(out Poolable poolable) && go.TryGetComponent(out NGO_PoolingInitalize_Base poolingOBJ))
    //        {
    //            poolingOBJ_Path.Add((poolingOBJ.PoolingNGO_PATH, poolingOBJ.PoolingCapacity));
    //        }
    //    }
    //    return poolingOBJ_Path;
    //}
    public override NetworkObject ParticleNGO => throw new System.NotImplementedException();

    public override void SetInitalze(NetworkObject particleObj)
    {
        throw new System.NotImplementedException();
    }
}

