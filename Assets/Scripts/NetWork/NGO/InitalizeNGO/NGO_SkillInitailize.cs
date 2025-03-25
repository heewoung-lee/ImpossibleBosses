using Unity.Netcode;
using UnityEngine;

public class NGO_SkillInitailize : NGO_InitailizeBase
{
    NetworkObject _skillParticleOBJ;

    public override NetworkObject SpawnNgo => _skillParticleOBJ;

    public override void SetInitalze(NetworkObject obj)
    {
        _skillParticleOBJ = obj;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

}
