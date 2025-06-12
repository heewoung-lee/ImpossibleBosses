using NetWork.BaseNGO;

namespace NetWork.NGO.InitializeNGO.EffectVFX
{
    public class NgoLevelUpInitalize : NgoPoolingInitalizeBase
    {
        public override string PoolingNgoPath => "Prefabs/Player/SkillVFX/Level_up";
        public override int PoolingCapacity => 5;
    }
}
