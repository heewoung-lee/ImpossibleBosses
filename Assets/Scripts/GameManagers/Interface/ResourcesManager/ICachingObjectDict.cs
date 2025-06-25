using UnityEngine;

namespace GameManagers.Interface.ResourcesManager
{
    public interface ICachingObjectDict
    {
        public bool TryGet(string path, out GameObject go);
        public void AddData(string path, GameObject go);
        public void OverwriteData(string path, GameObject go);
    }
}
