using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using Scene.ZenjectInstaller;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    internal class ResourceManager : IResourcesLoader,IInstantiate,IDestroyObject,IRegistrar<ICachingObjectDict>
    {      
        [Inject] private RelayManager _relayManager;

        public DiContainer CurrentContainer
        {
            get
            {
                //씬 컨텍스트가 있으면 그 컨테이너
                SceneContextRegistry registry = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>();
                SceneContext sceneCtx = registry.SceneContexts.FirstOrDefault(); // 한 씬만 가정
                if (sceneCtx != null)
                    return sceneCtx.Container;

                //없으면 전역 컨테이너
                return ProjectContext.Instance.Container;
            }
        }
        private ICachingObjectDict _cachingObjectDict;
        
        public void Register(ICachingObjectDict sceneContext)
        {
            _cachingObjectDict = sceneContext;
        }

        public void Unregister(ICachingObjectDict sceneContext)
        {
            if (_cachingObjectDict == sceneContext)
            {
                _cachingObjectDict = null;
            }
        }
        
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


        public GameObject InstantiateByPath(string path, Transform parent = null)
        {

            if (string.IsNullOrEmpty(path))
                return null;
            
            
            if ( _cachingObjectDict?.TryGet(path, out GameObject cachedPrefab) == true)
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
                _cachingObjectDict?.OverwriteData(path, prefab);//주의점 대신에 경로에 대한 딕셔너리 키는 원본경로로 들어감
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
            GameObject go = InstantiatePrefab(prefab, parent);
            return go;
        }

        public GameObject InstantiateByObject(GameObject gameobject, Transform parent = null)
        {
            return InstantiatePrefab(gameobject, parent);
        }
        private GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
        {
            return CurrentContainer.InstantiatePrefab(prefab, parent).RemoveCloneText();
        }
        public T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = null;
            component = go.GetComponent<T>();
            if (component == null)
            {
                go.SetActive(false);
                component = CurrentContainer.InstantiateComponent<T>(go);
                go.SetActive(true);
            }
            return component;
        }


        private bool IsCheckNetworkPrefab(GameObject prefab)
        {
            if (_relayManager.NetworkManagerEx.IsListening && prefab.TryGetComponent(out NetworkObject ngo))
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
                if (_relayManager.NetworkManagerEx.IsListening && poolable.TryGetComponent(out NetworkObject ngo))
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

    }
}
