using System.Collections.Generic;
using UnityEngine;

namespace GameManagers.Interface.Resources_Interface
{
    public interface IInstantiate
    {
        Dictionary<string, GameObject> CachingPoolableObject { get; }
        public GameObject Instantiate(string path, Transform parent = null);

    }
}