using System;
using Unity.Netcode;
using UnityEngine;
using static PlaySceneMockUnitTest;
using static UnityEngine.Rendering.DebugUI;

public class NGO_LevelUP_Initalize : NGO_PoolingInitalize_Base
{
    NetworkObject _particleNGO;
    NetworkObject _targetNGO;

    public override NetworkObject TargetNgo => _targetNGO;
    public override NetworkObject ParticleNGO => _particleNGO;


    public override string PoolingNGO_PATH => "Prefabs/Player/SkillVFX/Level_up";

    public override int PoolingCapacity => 5;

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
