using System;
using Unity.Netcode;
using UnityEngine;

public class StoneGolem_Skill1_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator";

    public override int PoolingCapacity => 50;

}
