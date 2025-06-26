using UnityEngine;

namespace GameManagers.Interface.BufferManager
{
    public interface IDetectObject
    {
        public Collider[] DetectedPlayers();
        public Collider[] DetectedOther(params string[] layerName);
    }
}
