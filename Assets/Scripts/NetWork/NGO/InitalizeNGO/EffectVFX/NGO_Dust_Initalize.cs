using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_Dust_Initalize : NGO_PoolingInitalize_Base
{
    NetworkObject _particleNGO;
    NetworkObject _targetNGO;
    public override string PoolingNGO_PATH => "Prefabs/Paticle/AttackEffect/Dust_Paticle";

    public override int PoolingCapacity => 400;

    public override NetworkObject TargetNgo => _targetNGO;

    public override NetworkObject ParticleNGO => _particleNGO;

    public override void SetInitalze(NetworkObject particleOBJ)
    {
        _particleNGO = particleOBJ;
    }
    public override void SetTargetInitalze(NetworkObject targetNgo)
    {
        _targetNGO = targetNgo;
    }
    public override void StartParticle(string path, float duration, Action<GameObject> positionAndBehaviorSetterEvent)
    {
        base.StartParticle(path, duration, positionAndBehaviorSetterEvent);
    }
}
