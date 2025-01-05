using System;

public class SlashState : IState
{
    public bool lockAnimationChange => true;

    public event Action UpdateStateEvent;

    public SlashState(Action slashState)
    {
        UpdateStateEvent += slashState;
    }

    public void UpdateState()
    {
        UpdateStateEvent?.Invoke();
    }
}