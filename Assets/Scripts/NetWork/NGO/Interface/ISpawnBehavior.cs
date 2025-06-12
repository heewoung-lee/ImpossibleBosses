using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NetWork.NGO.Interface
{
    public interface ISpawnBehavior
    {
        public void SpawnObjectToLocal(in SpawnParamBase param,string runtimePath = null);
    }
}