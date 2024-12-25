using System;

public interface IMoveableState
{

    public event Action UpdateStateEvent;
    public void UpdateState();
}