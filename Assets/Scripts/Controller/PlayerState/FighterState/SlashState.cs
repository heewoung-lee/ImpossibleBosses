using System;
using Controller.ControllerStats;

namespace Controller.PlayerState.FighterState
{
    public class SlashState : IState
    {
        public bool LockAnimationChange => true;

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
}