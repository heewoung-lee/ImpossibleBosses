using System;
using Unity.Netcode;
using UnityEngine;
using static PlaySceneMockUnitTest;
using static UnityEngine.Rendering.DebugUI;

public class NGO_FigherSkill_Roar_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Player/SkillVFX/Aura_Roar";

    public override int PoolingCapacity => 5;
}
