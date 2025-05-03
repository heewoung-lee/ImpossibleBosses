using System;
using Unity.Netcode;
using UnityEngine;
using static PlaySceneMockUnitTest;
using static UnityEngine.Rendering.DebugUI;

public class NGO_LevelUP_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Player/SkillVFX/Level_up";
    public override int PoolingCapacity => 5;
}
