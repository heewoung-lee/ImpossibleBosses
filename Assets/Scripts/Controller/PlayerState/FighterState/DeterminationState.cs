using System;

public class DeterminationState : IState
{
    public bool lockAnimationChange => true;

    public event Action UpdateStateEvent;
    public DeterminationState(Action determinationState)
    {
        UpdateStateEvent += determinationState;
    }
    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}