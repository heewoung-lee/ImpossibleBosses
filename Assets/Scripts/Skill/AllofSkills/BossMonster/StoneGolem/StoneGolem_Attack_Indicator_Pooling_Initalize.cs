using Unity.Netcode;
using UnityEngine;

public class StoneGolem_Attack_Indicator_Pooling_Initalize : NGO_PoolingInitalize_Base
{
    NetworkObject _particleNGO;
    NetworkObject _targetNGO;
    public override string PoolingNGO_PATH => "Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator";
    public override int PoolingCapacity => 5;

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

}
