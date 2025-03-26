using System;
using Unity.Netcode;
using UnityEngine;

public class LevelUP_VFX_NGO : NGO_Skill_Initailize_Base
{
    public override NetworkObject TargetNgo => _targetNgo;

    public override NetworkObject SpawnNgo => _spawnNgo;


    private NetworkObject _targetNgo;
    private NetworkObject _spawnNgo;

    public override void SetInitalze(NetworkObject obj)
    {
        _spawnNgo = obj;
    }

    public override void SetTargetInitalze(NetworkObject targetNgo)
    {
        _targetNgo = targetNgo;
    }

    public override void InvokeSkill(string path, float duration, Action<GameObject> positionAndBehaviorSetterEvent)
    {
        base.InvokeSkill(path, duration, positionAndBehaviorSetterEvent);
    }
}
