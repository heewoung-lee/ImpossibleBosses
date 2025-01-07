using System;

public class TauntState : IState
{
    public bool lockAnimationChange => true;

    public event Action UpdateStateEvent;

    public TauntState(Action tauntState)
    {
        UpdateStateEvent += tauntState;
    }

    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}