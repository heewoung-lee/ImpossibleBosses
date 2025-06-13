using NetWork.BaseNGO;

namespace Skill.AllofSkills.BossMonster.StoneGolem
{
    public class StoneGolemAttackIndicatorPoolingInitalize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator";
        public override int PoolingCapacity => 5;

    }
}
