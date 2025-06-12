using System;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;

public class StoneGolem_Attack_Indicator_Pooling_Initalize : NgoPoolingInitalizeBase
{
    public override string PoolingNgoPath => "Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator";
    public override int PoolingCapacity => 5;

}
