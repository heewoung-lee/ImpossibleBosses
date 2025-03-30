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
    HashSet<GameObject> m_Prefabs = new HashSet<GameObject>();

    Dictionary<GameObject, ObjectPool<NetworkObject>> m_PooledObjects = new Dictionary<GameObject, ObjectPool<NetworkObject>>();

    public Dictionary<GameObject, ObjectPool<NetworkObject>> PooledObjects => m_PooledObjects;
    public override void OnNetworkSpawn()
    {
    }

    public override void OnNetworkDespawn()
    {
        // 디스폰 될때 모든 프리펩들이 등록을 취소하고 없어져야 함.
        // Unregisters all objects in PooledPrefabsList from the cache.
        foreach (var prefab in m_Prefabs)
        {
            // Unregister Netcode Spawn handlers
            m_PooledObjects[prefab].Clear(); // <-- 여기서 ActionOnDestroy 호출됨!
        }
        m_PooledObjects.Clear();
        m_Prefabs.Clear();
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
    /// <param name="prefab"></param>
    /// <param name="position">The position to spawn the object at.</param>
    /// <param name="rotation">The rotation to spawn the object with.</param>
    /// <returns></returns>
    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var networkObject = m_PooledObjects[prefab].Get();

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
        m_PooledObjects[prefab].Release(networkObject);
    }

    /// <summary>
    /// Builds up the cache for a prefab.
    /// </summary>
    public void RegisterPrefabInternal(GameObject prefab, int prewarmCount = 5)
    {
        if(Managers.RelayManager.NetworkManagerEx.GetNetworkPrefabOverride(prefab) == null)
        {
            Debug.Log($"{prefab.name} is not registed the NetworkManager");
            return;
        }

        NetworkObject CreateFunc()
        {
            NetworkObject ngo = Instantiate(prefab).GetComponent<NetworkObject>();
            ngo.transform.SetParent(Managers.VFX_Manager.VFX_Root_NGO);
            return ngo;
            //TODO: 아침에 와서 네트워크 풀링 다시 작성하기
            //해쉬셋 필요없을 것 같고, 딕셔너리도 내가 만든 Instantiate를 사용하기위해서 Key를 전부 String(Path) 으로 바꿔야함
            //위에 호스트가 생성한다음 부모 위치로 초기화 시키는 작업이 필요
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

        m_Prefabs.Add(prefab);

        // Create the pool
        m_PooledObjects[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

        // Populate the pool
        var prewarmNetworkObjects = new List<NetworkObject>();
        for (var i = 0; i < prewarmCount; i++)
        {
            prewarmNetworkObjects.Add(m_PooledObjects[prefab].Get());
        }
        foreach (var networkObject in prewarmNetworkObjects)
        {
            m_PooledObjects[prefab].Release(networkObject);
        }
      
    }
}