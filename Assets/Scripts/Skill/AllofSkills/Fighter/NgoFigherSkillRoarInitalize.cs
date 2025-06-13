using NetWork.BaseNGO;

namespace Skill.AllofSkills.Fighter
{
    public class NgoFigherSkillRoarInitalize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Aura_Roar";

        public override int PoolingCapacity => 5;
    }
}
