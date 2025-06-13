using NetWork.BaseNGO;

namespace Skill.AllofSkills.Fighter
{
    public class NgoFigherSkillSlashInitalize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Fighter_Slash";
        public override int PoolingCapacity => 5;
        protected override void StartParticleOption()
        {
            base.StartParticleOption();
            ParticleNgo.transform.rotation = TargetNgo.transform.rotation;
        }
    }
}
