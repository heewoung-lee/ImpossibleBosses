using Unity.Netcode;
using UnityEngine;

public abstract class NGO_InitailizeBase : NetworkBehaviour
{
    public abstract NetworkObject SpawnNgo { get; }
    public abstract void SetInitalze(NetworkObject obj);
}
