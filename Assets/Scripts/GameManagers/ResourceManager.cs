using System.Collections;
using System.Collections.Generic;
using GameManagers.Interface.Resources_Interface;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    internal class ResourceManager : IManagerIResettable,IResourcesLoader,IInstantiate,IDestroyObject
    {
        [Inject] private DiContainer  _container;
            
        Dictionary<string, GameObject> _cachingPoolableObject = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> CachingPoolableObject => _cachingPoolableObject;
        
        
        
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
            loadItem = Load<T>(path);

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
                if (IsCheckNetworkPrefab(cachedPrefab))
                {
                    return Managers.NgoPoolManager.Pop(path);
                }
                else
                {
                    return Managers.PoolManager.Pop(cachedPrefab, parent).gameObject;
                }
            }

            GameObject prefab = Load<GameObject>(path); // 먼저 path를 시도 하고 없으면 prefab붙여서 시도
            if(prefab == null)
            {
                string prefabPath = "Prefabs/" + path;
                prefab = Load<GameObject>(prefabPath);
            }

            if (prefab == null)
            {
                Debug.Log($"Failed to Load Object Path:{path}");
                return null;
            }

            if (prefab.GetComponent<Poolable>() != null)
            {
                _cachingPoolableObject[path] = prefab;//주의점 대신에 경로에 대한 딕셔너리 키는 원본경로로 들어감
                if (IsCheckNetworkPrefab(prefab))
                {
                    return Managers.NgoPoolManager.Pop(path, parent);
                }
                else
                {
                    return Managers.PoolManager.Pop(prefab, parent).gameObject;
                }
            }
            //GameObject go = Object.Instantiate(prefab, parent).RemoveCloneText();
            GameObject go = _container.InstantiatePrefab(prefab, parent).RemoveCloneText();
            return go;
        }

        private bool IsCheckNetworkPrefab(GameObject prefab)
        {
            if (Managers.RelayManager.NetworkManagerEx.IsListening && prefab.TryGetComponent(out NetworkObject ngo))
            {
                return true;
            }
            return false;
        }
        public void DestroyObject(GameObject go, float duration = 0)
        {
            if (go == null)
                return;

            Poolable poolable = go.GetComponent<Poolable>();

            if (poolable != null)
            {
                if (Managers.RelayManager.NetworkManagerEx.IsListening && poolable.TryGetComponent(out NetworkObject ngo))
                {
                    Managers.ManagersStartCoroutine(PrefabPushCoroutine(() =>
                    {
                        Managers.NgoPoolManager.Push(ngo);
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
}
