using Unity.Netcode;
using UnityEngine;

public class NGO_SkillInitailize : NGO_InitailizeBase
{
    GameObject _skillParticleOBJ;
    public override void SetInitalze(NetworkObject obj)
    {
        _skillParticleOBJ = obj.gameObject;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

}
