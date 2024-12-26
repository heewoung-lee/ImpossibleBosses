using System;

public interface IState
{

    public event Action UpdateStateEvent;
    public void UpdateState();
}