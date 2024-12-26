using System;

public class PickUpState : IState
{
    public event Action UpdateStateEvent;
    public PickUpState(Action pickUpState)
    {
        UpdateStateEvent += pickUpState;
    }
    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}
