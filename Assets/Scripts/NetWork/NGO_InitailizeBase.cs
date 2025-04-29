using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public abstract class NGO_InitailizeBase : NetworkBehaviour
{
    public abstract NetworkObject ParticleNGO { get; }
    public abstract void SetInitalze(NetworkObject particleObj);
}
