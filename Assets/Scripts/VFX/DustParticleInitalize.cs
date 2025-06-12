using GameManagers;
using NetWork;
using NetWork.NGO.Interface;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;



public class DustParticleInitalize : Poolable,ISpawnBehavior
{
    public void SpawnObjectToLocal(in SpawnParamBase param,string path = null)
    {
        Managers.VFXManager.GenerateParticle(path, param.ArgPosVector3, param.ArgFloat);
    }
}
