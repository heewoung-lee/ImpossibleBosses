using System;
using Controller.ControllerStats;

public class RoarState : IState
{
    public bool LockAnimationChange => true;

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