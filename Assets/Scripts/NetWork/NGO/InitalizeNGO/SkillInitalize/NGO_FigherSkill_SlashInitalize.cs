using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_SlashInitalize : NGO_Skill_Initailize_Base
{
    NetworkObject _particleNGO;
    NetworkObject _targetNGO;

    public override NetworkObject TargetNgo => _targetNGO;
    public override NetworkObject SpawnNgo => _particleNGO;

    public override void SetInitalze(NetworkObject obj)
    {
        _particleNGO = obj;
    }
    public override void SetTargetInitalze(NetworkObject targetNgo)
    {
        _targetNGO = targetNgo;
    }

    public override void InvokeSkill(string path, float duration, Action<GameObject> positionAndBehaviorSetterEvent)
    {
        base.InvokeSkill(path, duration, positionAndBehaviorSetterEvent);
        _particleNGO.transform.rotation = _targetNGO.transform.rotation;
    }
}
