using BaseStates;
using System;

public class BossSkill1State : IState
{
    public bool lockAnimationChange => false;

    public event Action UpdateStateEvent;
    public BossSkill1State(Action bossSkill1State)
    {
        UpdateStateEvent += bossSkill1State;
    }
    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}
