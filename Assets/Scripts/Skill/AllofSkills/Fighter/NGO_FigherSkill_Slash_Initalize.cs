using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_Slash_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Player/SkillVFX/Fighter_Slash";
    public override int PoolingCapacity => 5;
    protected override void StartParticleOption()
    {
        base.StartParticleOption();
        ParticleNGO.transform.rotation = TargetNgo.transform.rotation;
    }
}
