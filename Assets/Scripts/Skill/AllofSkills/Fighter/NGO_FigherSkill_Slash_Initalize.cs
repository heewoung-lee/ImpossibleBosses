using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_Slash_Initalize : NGO_PoolingInitalize_Base
{
    NetworkObject _particleNGO;
    NetworkObject _targetNGO;

    public override NetworkObject TargetNgo => _targetNGO;
    public override NetworkObject ParticleNGO => _particleNGO;

    public override string PoolingNGO_PATH => "Prefabs/Player/SkillVFX/Fighter_Slash";

    public override int PoolingCapacity => 5;

    public override void SetInitalze(NetworkObject obj)
    {
        _particleNGO = obj;
    }
    public override void SetTargetInitalze(NetworkObject targetNgo)
    {
        _targetNGO = targetNgo;
    }

    public override void StartParticle(string path, float duration, Action<GameObject> positionAndBehaviorSetterEvent)
    {
        base.StartParticle(path, duration, positionAndBehaviorSetterEvent);
        _particleNGO.transform.rotation = _targetNGO.transform.rotation;
    }
}
