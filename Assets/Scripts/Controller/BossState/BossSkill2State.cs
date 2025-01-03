using BaseStates;
using System;

public class BossSkill2State : IState
{
    public bool lockAnimationChange => false;

    public event Action UpdateStateEvent;
    public BossSkill2State(Action bossSkill2State)
    {
        UpdateStateEvent += bossSkill2State;
    }
    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}
