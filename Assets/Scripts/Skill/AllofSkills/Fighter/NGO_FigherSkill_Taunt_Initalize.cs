using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_Taunt_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Player/SkillVFX/Taunt_Player";

    public override int PoolingCapacity => 5;
}
