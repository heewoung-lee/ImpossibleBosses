using Unity.Netcode;
using UnityEngine;

public class NGO_VFXInitalize : NGO_InitailizeBase
{
    public override void SetInitalze(NetworkObject obj)
    {
       Managers.VFX_Manager.Set_VFX_Root_NGO(obj);
    }
}
