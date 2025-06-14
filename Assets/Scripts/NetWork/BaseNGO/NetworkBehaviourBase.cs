using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Util;
using Object = UnityEngine.Object;

namespace NetWork.BaseNGO
{
    public abstract class NetworkBehaviourBase : NetworkBehaviour
    {
        Dictionary<Type, Object[]> _bindDictionary = new Dictionary<Type, Object[]>();
        protected abstract void StartInit();
        protected abstract void AwakeInit();

        protected virtual void OnEnableInit() { }
        protected virtual void OnDisableInit() { }

        private void OnEnable()
        {
            OnEnableInit();
        }

        private void OnDisable()
        {
            OnDisableInit();
        }

        private void Awake()
        {
            AwakeInit();
        }

        private void Start()
        {
            StartInit();
        }
        protected void Bind<T>(Type type) where T : Object
        {

            if (type.IsEnum == false)
                return;

            string[] names = Enum.GetNames(type);
            Object[] objects = new Object[names.Length];
            objects = FindObjects<T>(objects, 0, names.Length, names);

            _bindDictionary.Add(typeof(T), objects);
        }



        private Object[] FindObjects<T>(Object[] objects, int startIndex, int endIndex, string[] names) where T : Object
        {
            Object[] newObjects = objects;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (typeof(T) == typeof(GameObject))
                {
                    newObjects[i] = Utill.FindChild(gameObject, names[i], true);
                }
                else
                {
                    newObjects[i] = Utill.FindChild<T>(gameObject, names[i], true);
                }
            }
            return newObjects;
        }


        protected T Get<T>(int idx) where T : Object
        {
            Object[] objects = null;

            if (_bindDictionary.TryGetValue(typeof(T), out objects) == false)
            {
                Debug.LogError($"not Found Object{typeof(T)}");
                return null;
            }
            return objects[idx] as T;
        }

    }
}