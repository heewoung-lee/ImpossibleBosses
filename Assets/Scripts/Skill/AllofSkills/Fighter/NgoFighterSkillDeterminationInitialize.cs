using NetWork.BaseNGO;

namespace Skill.AllofSkills.Fighter
{
    public class NgoFighterSkillDeterminationInitialize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Shield_Determination";

        public override int PoolingCapacity => 5;

    }
}
