using NetWork.BaseNGO;

namespace Skill.AllofSkills.Fighter
{
    public class NgoFigherSkillTauntInitalize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Taunt_Player";

        public override int PoolingCapacity => 5;
    }
}
