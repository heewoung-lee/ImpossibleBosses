using UnityEngine;

namespace GameManagers.Interface.ResourcesManager
{
    public interface IDestroyObject
    {
        public void DestroyObject(GameObject go, float duration = 0);
    }
}