using System.Collections.Generic;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    public class PoolManager : IManagerInitializable,IManagerIResettable
    {
        private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        private Transform _rootTransform;
        #region Pool
        public class Pool
        {
            [Inject] IInstantiate _instantiate;
            [Inject] SceneManagerEx _sceneManagerEx;
            public GameObject Original { get; private set; }
            Stack<Poolable> _poolStack = new Stack<Poolable>();
            public Transform Root { get; private set; }


            public void Init(GameObject gameObject,int count)
            {
                Original = gameObject;
                Root = new GameObject().transform;
                Root.gameObject.name = $"{gameObject.name}_Root";

                for(int i = 0; i < count; i++)
                {
                    Push(Create());
                }

            }

            public Poolable Create()
            {
                GameObject go = _instantiate.InstantiateByObject(Original);
                go.name = Original.name;
                return _instantiate.GetOrAddComponent<Poolable>(go);
            }

            public void Push(Poolable item)
            {
                if (item == null)
                    return;

                item.transform.SetParent(Root,item.WorldPositionStays);
                item.transform.gameObject.SetActive(false);
                item.IsUsing = false;

                _poolStack.Push(item);
            }

            public Poolable Pop(Transform parent = null)
            {
                Poolable popitem;
                if (_poolStack.Count > 0)
                    popitem = _poolStack.Pop();
                else
                    popitem = Create();

                if(parent == null)
                    popitem.transform.SetParent(_sceneManagerEx.GetCurrentScene.transform);


                popitem.transform.SetParent(parent);//parent가 Null이라면 BaseScene에 하위에 있는 자식들이 전부 다시 부모가 없게 되어버림
                popitem.IsUsing = true;
                popitem.gameObject.SetActive(true);

                return popitem;
            }

        }

        #endregion
        public void Init()
        {
            GameObject go = GameObject.Find("@Pool_Root");
            if (go == null)
                go = new GameObject() { name = "@Pool_Root" };

            _rootTransform = go.transform;
            Object.DontDestroyOnLoad(go);
        }

        public void CreatePool(GameObject original,int count = 5)
        {
            if (original == null)
                return;

            Pool pool = new Pool();
            pool.Init(original,count);
            pool.Root.parent = _rootTransform;

            _pools.Add(original.name, pool);
        }


        public void Push(Poolable poolable)
        {
            if(poolable == null) return;

            string objectName = poolable.gameObject.name;
            if(_pools.ContainsKey(objectName) == false)
            {
                Object.Destroy(poolable.gameObject);
                return;
            }

            _pools[objectName].Push(poolable);
        }

        public Poolable Pop(GameObject go, Transform parent = null)
        {
            if (_pools.ContainsKey(go.name) == false)
                CreatePool(go);

            return _pools[go.name].Pop(parent); 
        }


        public GameObject GetOriginal(string name)
        {
            if (_pools.ContainsKey(name) == false)
                return null;

            return _pools[name].Original;
        }

        public void Clear()
        {
            foreach(Transform child in _rootTransform)
                GameObject.Destroy(child.gameObject);

            _pools.Clear();
        }
    }
}