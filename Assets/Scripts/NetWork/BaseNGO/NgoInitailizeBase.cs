using Unity.Netcode;
using UnityEngine;

namespace NetWork.BaseNGO
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NgoInitailizeBase : NetworkBehaviour
    {
        public abstract NetworkObject ParticleNgo { get; }
        public abstract void SetInitalze(NetworkObject particleObj);
    }
}
