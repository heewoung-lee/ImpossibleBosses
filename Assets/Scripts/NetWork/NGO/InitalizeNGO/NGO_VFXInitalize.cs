using GameManagers;
using Unity.Netcode;
using UnityEngine;

public class NGO_VFXInitalize : NGO_InitailizeBase
{
    NetworkObject _vfx_Root_NGO;

    public override NetworkObject ParticleNGO => _vfx_Root_NGO;

    public override void SetInitalze(NetworkObject obj)
    {
       Managers.VFXManager.Set_VFX_Root_NGO(obj);
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.VFXManager.Set_VFX_Root_NGO(gameObject.GetComponent<NetworkObject>());
    }
}
