using System.Collections.Generic;
using UnityEngine;

namespace GameManagers
{
    public class CachingObjectDictManager:ICachingObjectDict
    {
        private Dictionary<string,GameObject> _cachingobjectDict = new Dictionary<string,GameObject>(); 
        public Dictionary<string, GameObject> CachingObjectDict => _cachingobjectDict;  
    }
}
