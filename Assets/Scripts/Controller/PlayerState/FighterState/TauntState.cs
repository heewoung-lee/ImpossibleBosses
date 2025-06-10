using System;
using Controller.ControllerStats;

namespace Controller.PlayerState.FighterState
{
    public class TauntState : IState
    {
        public bool LockAnimationChange => true;

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
}