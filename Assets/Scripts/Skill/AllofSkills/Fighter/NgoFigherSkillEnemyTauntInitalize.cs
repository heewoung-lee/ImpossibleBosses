using NetWork.BaseNGO;

namespace Skill.AllofSkills.Fighter
{
    public class NgoFigherSkillEnemyTauntInitalize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Taunt_Enemy";

        public override int PoolingCapacity => 5;

    }
}
