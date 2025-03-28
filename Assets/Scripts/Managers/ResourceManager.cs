using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
    
    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T> (path);
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

        GameObject prefab = Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.Log($"Failed to Load Object Path:{path}");
        }


        if (prefab.GetComponent<Poolable>() != null)
        {
            if (Managers.RelayManager.NetworkManagerEx.IsListening && prefab.TryGetComponent(out NetworkObject ngo))
            {
                //return Managers.NGO_PoolManager.Pop(ngo, parent);
            }
            else
            {
                return Managers.PoolManager.Pop(prefab, parent).gameObject;
            }
        }


        GameObject go = Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);


        return go;
    }

    public GameObject InstantiatePrefab(string path,Transform parent = null)
    {
        return Instantiate("Prefabs/"+path, parent);
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
                //Managers.NGO_PoolManager.Des

            }
            else
            {
                Managers.ManagersStartCoroutine(PushCoroutine(poolable, duration));
            }
            return;
        }
        Object.Destroy(go, duration);
    }


    IEnumerator PushCoroutine(Poolable poolable,float duration)
    {
        if(duration <= 0)
        {
            Managers.PoolManager.Push(poolable);
        }
        else
        {
            yield return new WaitForSeconds(duration);
            Managers.PoolManager.Push(poolable);
        }
    }

}
