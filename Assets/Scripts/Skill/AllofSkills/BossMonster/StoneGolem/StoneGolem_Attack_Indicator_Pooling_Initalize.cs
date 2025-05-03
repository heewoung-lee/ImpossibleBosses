using System;
using Unity.Netcode;
using UnityEngine;

public class StoneGolem_Attack_Indicator_Pooling_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator";
    public override int PoolingCapacity => 5;

}
