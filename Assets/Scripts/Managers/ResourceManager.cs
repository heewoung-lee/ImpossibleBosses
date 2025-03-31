using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : IManagerIResettable
{
    Dictionary<string, GameObject> _cachingPoolableObject = new Dictionary<string, GameObject>();

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }


    public bool TryGetLoad<T>(string path, out T loadItem) where T : UnityEngine.Object
    {
        loadItem = Managers.ResourceManager.Load<T>(path);

        if (loadItem == null)
            return false;
        else
            return true;
    }
    public GameObject Instantiate(string path, Transform parent = null)
    {

        if (string.IsNullOrEmpty(path))
            return null;

        if (_cachingPoolableObject.TryGetValue(path, out GameObject cachedPrefab))
        {
            if (isCheckNetworkPrefab(cachedPrefab))
            {
                return Managers.NGO_PoolManager.Pop(cachedPrefab);
            }
            else
            {
                return Managers.PoolManager.Pop(cachedPrefab, parent).gameObject;
            }
        }

        GameObject prefab = Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.Log($"Failed to Load Object Path:{path}");
            return null;
        }

        if (prefab.GetComponent<Poolable>() != null)
        {
            _cachingPoolableObject[path] = prefab;
            if (isCheckNetworkPrefab(prefab))
            {
                return Managers.NGO_PoolManager.Pop(prefab, parent);
            }
            else
            {
                return Managers.PoolManager.Pop(prefab, parent).gameObject;
            }
        }

        GameObject go = Object.Instantiate(prefab, parent).RemoveCloneText();

        return go;
    }
    private GameObject RemoveCloneText(GameObject go)
    {
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }
    private bool isCheckNetworkPrefab(GameObject prefab)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsListening && prefab.TryGetComponent(out NetworkObject ngo))
        {
            return true;
        }
        return false;
    }


    public GameObject InstantiatePrefab(string path, Transform parent = null)
    {
        return Instantiate("Prefabs/" + path, parent);
    }

    public void DestroyObject(GameObject go, float duration = 0)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();

        if (poolable != null)
        {
            if (Managers.RelayManager.NetworkManagerEx.IsListening && poolable.GetComponent<NetworkObject>())
            {
                Managers.ManagersStartCoroutine(PrefabPushCoroutine(() =>
                {
                    Managers.NGO_PoolManager.Push(go);
                }, duration));
            }
            else
            {
                Managers.ManagersStartCoroutine(PrefabPushCoroutine(() =>
                {
                    Managers.PoolManager.Push(poolable);
                }, duration));
            }
            return;
        }
        Object.Destroy(go, duration);
    }


    IEnumerator PrefabPushCoroutine(System.Action prefabPushEvent,float duration)
    {
        if (duration <= 0)
        {
            prefabPushEvent.Invoke();
         
        }
        else
        {
            yield return new WaitForSeconds(duration);
            prefabPushEvent.Invoke();
        }
    }
    public void Clear()
    {
        _cachingPoolableObject.Clear();
    }
}
