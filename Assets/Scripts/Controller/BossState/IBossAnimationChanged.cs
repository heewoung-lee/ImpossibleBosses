using Controller.ControllerStats;
using NetWork.Boss_NGO;

namespace Controller.BossState
{
    public interface IBossAnimationChanged //TODO: Action Task만 모아서 인터페이스가 아닌 상속구조로 만들것
    {
        public BossGolemAnimationNetworkController BossAnimNetworkController { get;}


        public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state);
    }
}
