using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

/// <summary>
/// Object Pool for networked objects, used for controlling how objects are spawned by Netcode. Netcode by default
/// will allocate new memory when spawning new objects. With this Networked Pool, we're using the ObjectPool to
/// reuse objects.
/// Boss Room uses this for projectiles. In theory it should use this for imps too, but we wanted to show vanilla spawning vs pooled spawning.
/// Hooks to NetworkManager's prefab handler to intercept object spawning and do custom actions.
/// </summary>
public class NetworkObjectPool : NetworkBehaviour
{
    Dictionary<string, ObjectPool<NetworkObject>> m_PooledObjects = new Dictionary<string, ObjectPool<NetworkObject>>();

    public Dictionary<string, ObjectPool<NetworkObject>> PooledObjects => m_PooledObjects;

    public override void OnNetworkDespawn()
    {
        // 디스폰 될때 모든 프리펩들이 등록을 취소하고 없어져야 함.
        // Unregisters all objects in PooledPrefabsList from the cache.
        foreach (string prefabPath in m_PooledObjects.Keys)
        {
            m_PooledObjects[prefabPath].Clear();
        }
        m_PooledObjects.Clear();
    }

    /// <summary>
    /// Gets an instance of the given prefab from the pool. The prefab must be registered to the pool.
    /// </summary>
    /// <remarks>
    /// To spawn a NetworkObject from one of the pools, this must be called on the server, then the instance
    /// returned from it must be spawned on the server. This method will then also be called on the client by the
    /// PooledPrefabInstanceHandler when the client receives a spawn message for a prefab that has been registered
    /// here.
    /// </remarks>
    /// <param name="prefabPath"></param>
    /// <param name="position">The position to spawn the object at.</param>
    /// <param name="rotation">The rotation to spawn the object with.</param>
    /// <returns></returns>
    public NetworkObject GetNetworkObject(string prefabPath, Vector3 position, Quaternion rotation)
    {
        var networkObject = m_PooledObjects[prefabPath].Get();

        var noTransform = networkObject.transform;
        noTransform.position = position;
        noTransform.rotation = rotation;

        return networkObject;
    }

    /// <summary>
    /// Return an object to the pool (reset objects before returning).
    /// </summary>
    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        Poolable poolable = prefab.GetComponent<Poolable>();
        m_PooledObjects[poolable.PollablePath].Release(networkObject);
    }

    /// <summary>
    /// Builds up the cache for a prefab.
    /// </summary>
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
            NetworkObject ngo = Instantiate(prefab, Managers.VFX_Manager.VFX_Root_NGO).RemoveCloneText().GetComponent<NetworkObject>();
            Debug.Log($"클라가 여길 거치나?{prefabPath}");
            ngo.GetComponent<Poolable>().SetPoolableDirectory(prefabPath);
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
        // Create the pool
        m_PooledObjects[prefabPath] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

        // Populate the pool
        var prewarmNetworkObjects = new List<NetworkObject>();
        for (var i = 0; i < prewarmCount; i++)
        {
            prewarmNetworkObjects.Add(m_PooledObjects[prefabPath].Get());
        }
        foreach (var networkObject in prewarmNetworkObjects)
        {
            m_PooledObjects[prefabPath].Release(networkObject);
        }

    }
}