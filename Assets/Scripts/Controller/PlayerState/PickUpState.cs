using System;
using Controller.ControllerStats;

namespace Controller.PlayerState
{
    public class PickUpState : IState
    {
        public bool LockAnimationChange => true;

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
}
