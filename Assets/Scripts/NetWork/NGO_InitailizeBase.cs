using Unity.Netcode;
using UnityEngine;

public abstract class NGO_InitailizeBase : NetworkBehaviour
{
    public abstract void SetInitalze(NetworkObject obj);
}
