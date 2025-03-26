using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_Slash_Initalize : NGO_Skill_Initailize_Base
{
    NetworkObject _particleNGO;
    NetworkObject _targetNGO;

    public override NetworkObject TargetNgo => _targetNGO;
    public override NetworkObject ParticleNGO => _particleNGO;

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
