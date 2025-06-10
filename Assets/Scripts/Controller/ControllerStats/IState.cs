using System;

namespace Controller.ControllerStats
{
    public interface IState
    {

        public event Action UpdateStateEvent;
        public void UpdateState();

        public bool LockAnimationChange { get; }
    }
}