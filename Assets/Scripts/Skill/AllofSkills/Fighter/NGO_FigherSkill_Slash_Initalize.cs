using System;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;

public class NGO_FigherSkill_Slash_Initalize : NgoPoolingInitalizeBase
{
    public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Fighter_Slash";
    public override int PoolingCapacity => 5;
    protected override void StartParticleOption()
    {
        base.StartParticleOption();
        ParticleNgo.transform.rotation = TargetNgo.transform.rotation;
    }
}
