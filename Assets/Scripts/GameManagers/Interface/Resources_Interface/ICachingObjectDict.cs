using System.Collections.Generic;
using UnityEngine;

namespace GameManagers.Interface.Resources_Interface
{
    public interface ICachingObjectDict
    {
        public bool TryGet(string path, out GameObject go);
        public void AddData(string path, GameObject go);
        public void OverwriteData(string path, GameObject go);
    }
}
