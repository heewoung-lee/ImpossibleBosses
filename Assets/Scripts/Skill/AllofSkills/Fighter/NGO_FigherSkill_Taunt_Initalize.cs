using System;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_Taunt_Initalize : NgoPoolingInitalizeBase
{
    public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Taunt_Player";

    public override int PoolingCapacity => 5;
}
