using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;



public class DustParticleInitalize : Poolable,ISpawnBehavior
{
    public void SpawnObjectToLocal(in SpawnParamBase param,string path)
    {
        Managers.VFX_Manager.GenerateParticle(path, param.argVector3, param.argFloat);
    }
}
