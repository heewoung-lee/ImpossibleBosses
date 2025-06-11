using GameManagers;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;



public class DustParticleInitalize : Poolable,ISpawnBehavior
{
    public void SpawnObjectToLocal(in SpawnParamBase param,string path = null)
    {
        Managers.VFX_Manager.GenerateParticle(path, param.argPosVector3, param.argFloat);
    }
}
