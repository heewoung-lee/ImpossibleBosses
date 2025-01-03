using System;

public class RoarState : IState
{
    public bool lockAnimationChange => true;

    public event Action UpdateStateEvent;
    public RoarState(Action roarState)
    {
        UpdateStateEvent += roarState;
    }
    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}