using BaseStates;
using System;

public class BossNormalAttackState : IState
{
    public bool lockAnimationChange => false;

    public event Action UpdateStateEvent;
    public BossNormalAttackState(Action bossNormalAttackState)
    {
        UpdateStateEvent += bossNormalAttackState;
    }
    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}
