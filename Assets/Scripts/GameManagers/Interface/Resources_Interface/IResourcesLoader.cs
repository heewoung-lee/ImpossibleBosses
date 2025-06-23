

using UnityEngine;

namespace GameManagers.Interface.Resources_Interface
{
    public interface IResourcesLoader
    {
        public void ResisterCacheManager(CachingObjectDictManager cachingDictManager);
        public T Load<T>(string path) where T : Object;
        public T[] LoadAll<T>(string path) where T : Object;
        public bool TryGetLoad<T>(string path, out T loadItem) where T : Object;
    }
}