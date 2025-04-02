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

    public NetworkObject GetNetworkObject(string prefabPath, Vector3 position, Quaternion rotation)
    {
        var networkObject = m_PooledObjects[prefabPath].Get();

        var noTransform = networkObject.transform;
        noTransform.position = position;
        noTransform.rotation = rotation;

        return networkObject;
    }

    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        prefab.TryGetComponent(out NGO_PoolingInitalize_Base poolingInitalize_Base);
        networkObject.transform.position = Vector3.zero;
        m_PooledObjects[poolingInitalize_Base.PoolingNGO_PATH].Release(networkObject);
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
            NetworkObject ngo = Instantiate(prefab,transform).RemoveCloneText().GetComponent<NetworkObject>();
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