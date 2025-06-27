using UnityEngine;

namespace GameManagers.Interface.ResourcesManager
{
    public interface IInstantiate
    {
        public GameObject InstantiateByPath(string path, Transform parent = null);
        public GameObject InstantiateByObject(GameObject gameobject, Transform parent = null);
        public T GetOrAddComponent<T>(GameObject go) where T : Component;
    }
}