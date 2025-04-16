using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class NetworkObjectPool : NetworkBehaviour
{
    Dictionary<string, ObjectPool<NetworkObject>> m_PooledObjects = new Dictionary<string, ObjectPool<NetworkObject>>();
    public Dictionary<string, ObjectPool<NetworkObject>> PooledObjects => m_PooledObjects;
    public override void OnNetworkDespawn()
    {
        foreach (string prefabPath in m_PooledObjects.Keys)
        {
            m_PooledObjects[prefabPath].Clear();
        }
        m_PooledObjects.Clear();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Managers.NGO_PoolManager.Set_NGO_Pool(this);

        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;


        foreach ((string, int) poolingPrefabInfo in Managers.NGO_PoolManager.AutoRegisterFromFolder())
        {
            //��ο� �°� Root������ ��
            GameObject pollingNgo_Root = Managers.ResourceManager.Instantiate("Prefabs/NGO/NGO_Polling_ROOT");
            if (pollingNgo_Root != null)
            {
               Managers.RelayManager.SpawnNetworkOBJ(pollingNgo_Root);
            }

            if (pollingNgo_Root.TryGetComponent(out NGO_Pool_RootInitailize initalilze))
            {
                initalilze.SetRootObjectName(poolingPrefabInfo.Item1);
            }
        }

        gameObject.RemoveCloneText();
    }

    public NetworkObject GetNetworkObject(string prefabPath, Vector3 position, Quaternion rotation)
    {
        NetworkObject networkObject = m_PooledObjects[prefabPath].Get();

        if (networkObject.TryGetComponent(out NGO_PoolingInitalize_Base poolingInitalize))
        {
            poolingInitalize.OnPoolGet();
        }
        Transform noTransform = networkObject.transform;
        noTransform.position = position;
        noTransform.rotation = rotation;

        return networkObject;
    }

    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        if (prefab.TryGetComponent(out NGO_PoolingInitalize_Base poolingInitalize_Base))
        {
            poolingInitalize_Base.OnPoolRelease();
            m_PooledObjects[poolingInitalize_Base.PoolingNGO_PATH].Release(networkObject);
        }
    }


    public void RegisterPrefabInternal(string prefabPath, int prewarmCount = 5)
    {
        GameObject prefab = Managers.ResourceManager.Load<GameObject>(prefabPath);

        if (Managers.RelayManager.NetworkManagerEx.GetNetworkPrefabOverride(prefab) == null)
        {
            Debug.Log($"{prefab.name} is not registed the NetworkManager");
            return;
        }

        NetworkObject CreateFunc()
        {
            NetworkObject ngo = Instantiate(prefab, Managers.NGO_PoolManager.Pool_NGO_Root_Dict[prefabPath]).RemoveCloneText().GetComponent<NetworkObject>();
            return ngo;
        }

        void ActionOnGet(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(true);
        }

        void ActionOnRelease(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(false);
        }

        void ActionOnDestroy(NetworkObject networkObject)
        {
            Destroy(networkObject.gameObject);
        }
        m_PooledObjects[prefabPath] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

        var prewarmNetworkObjects = new List<NetworkObject>();
        for (var i = 0; i < prewarmCount; i++)
        {
            prewarmNetworkObjects.Add(m_PooledObjects[prefabPath].Get());
        }
        foreach (var networkObject in prewarmNetworkObjects)
        {
            m_PooledObjects[prefabPath].Release(networkObject);
        }

        PooledPrefabInstanceHandler handler = new PooledPrefabInstanceHandler(this, prefabPath);

        Managers.RelayManager.NetworkManagerEx.PrefabHandler.AddHandler(prefab, handler);

    }
}
public class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    private readonly NetworkObjectPool m_NetworkObjectPool;
    private readonly string m_PrefabPath;

    public PooledPrefabInstanceHandler(NetworkObjectPool pool, string prefabPath)
    {
        m_NetworkObjectPool = pool;
        m_PrefabPath = prefabPath;
    }
    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return m_NetworkObjectPool.GetNetworkObject(m_PrefabPath, position, rotation);
    }
    public void Destroy(NetworkObject networkObject)
    {
        GameObject prefab = Managers.ResourceManager.Load<GameObject>(m_PrefabPath);
        m_NetworkObjectPool.ReturnNetworkObject(networkObject, prefab);
    }
}