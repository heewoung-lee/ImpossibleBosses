using System.Collections;
using UnityEngine;

namespace GameManagers.Interface.Resources_Interface
{
    public interface IDestroyObject
    {
        public void DestroyObject(GameObject go, float duration = 0);
    }
}