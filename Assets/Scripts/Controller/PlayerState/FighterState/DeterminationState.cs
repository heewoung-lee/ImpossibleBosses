using System;
using Controller.ControllerStats;

namespace Controller.PlayerState.FighterState
{
    public class DeterminationState : IState
    {
        public bool LockAnimationChange => true;

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
}