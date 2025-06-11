using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace GameManagers
{
    public class NgoPoolManager : IManagerIResettable
    {
        private NetworkObjectPool _ngoPool;
        public Dictionary<string, ObjectPool<NetworkObject>> PooledObjects => _ngoPool.PooledObjects;

        public NetworkObjectPool NgoPool => _ngoPool;

        private Dictionary<string, Transform> _poolNgoRootDict = new Dictionary<string, Transform>();
        public Dictionary<string, Transform> PoolNgoRootDict => _poolNgoRootDict;

        public void Set_NGO_Pool(NetworkObjectPool ngo)
        {
            _ngoPool = ngo;
        }
        public void Create_NGO_Pooling_Object()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost == false || _ngoPool != null)
                return;

            if (Managers.RelayManager.NgoRPCCaller == null)
            {
                Managers.RelayManager.SpawnRpcCallerEvent += SpawnNgoPolling;
            }
            else
            {
                SpawnNgoPolling();
            }

            void SpawnNgoPolling()
            {
                Managers.RelayManager.NgoRPCCaller.SpawnPrefabNeedToInitalizeRpc("Prefabs/NGO/NGO_Polling");
            }
        }
        public void SetPool_NGO_ROOT_Dict(string poolNgoPath,Transform rootTr)
        {
            _poolNgoRootDict.Add(poolNgoPath, rootTr);
        }
        public GameObject Pop(string prefabPath,Transform parantTr = null)
        {
            return _ngoPool.GetNetworkObject(prefabPath, Vector3.zero, Quaternion.identity).gameObject;
        }
        public void Push(NetworkObject ngo)
        {
            if (ngo == null)
                return;

            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                ngo.Despawn();
            }
        }

        public List<(string, int)> AutoRegisterFromFolder()
        {
            GameObject[] poolableNgoList = Managers.ResourceManager.LoadAll<GameObject>("Prefabs");
            List<(string, int)> poolingObjPath = new List<(string, int)>();
            foreach (GameObject go in poolableNgoList)
            {
                if (go.TryGetComponent(out Poolable poolable) && go.TryGetComponent(out NGO_PoolingInitalize_Base poolingObj))
                {
                    poolingObjPath.Add((poolingObj.PoolingNGO_PATH, poolingObj.PoolingCapacity));
                }
            }
            return poolingObjPath;
        }

        public void NGO_Pool_RegisterPrefab(string path,int capacity = 5)
        {
            _ngoPool.RegisterPrefabInternal(path, capacity);
        }

        public void Clear()
        {
            _poolNgoRootDict.Clear();
        }

    }
}