using System;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;
using static PlaySceneMockUnitTest;
using static UnityEngine.Rendering.DebugUI;

public class NGO_FigherSkill_Roar_Initalize : NgoPoolingInitalizeBase
{
    public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Aura_Roar";

    public override int PoolingCapacity => 5;
}
