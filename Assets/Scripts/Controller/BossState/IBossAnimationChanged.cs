using UnityEngine;

public interface IBossAnimationChanged //TODO: Action Task�� ��Ƽ� �������̽��� �ƴ� ��ӱ����� �����
{
    public BossGolemAnimationNetworkController BossAnimNetworkController { get;}


    public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state);
}
