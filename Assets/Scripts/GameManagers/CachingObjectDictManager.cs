using System;
using System.Collections.Generic;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using Scene.ZenjectInstaller;
using UnityEngine;
using Zenject;

namespace GameManagers
{
    public class CachingObjectDictManager : ICachingObjectDict, IDisposable
    {
        private readonly IRegistrar<ICachingObjectDict> _registrar;
        private readonly Dictionary<string, GameObject> _cachingobjectDict = new Dictionary<string, GameObject>();

        [Inject]
        public CachingObjectDictManager(IRegistrar<ICachingObjectDict> registrar)
        {
            _registrar = registrar;
            _registrar.Register(this);
        }
        public void Dispose()
        {
            _registrar.Unregister(this);
        }
        
        public bool TryGet(string path, out GameObject go)
        {
            if (_cachingobjectDict.TryGetValue(path, out go) == true)
            {
                return true;
            }

            return false;
        }

        public void AddData(string path, GameObject go)
        {
            _cachingobjectDict.Add(path, go);
        }

        public void OverwriteData(string path, GameObject go)
        {
            _cachingobjectDict[path] = go;
        }
        
    }
}