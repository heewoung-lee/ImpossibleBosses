using System;
using Controller.ControllerStats;

namespace Controller.BossState
{
    public class BossSkill2State : IState
    {
        public bool LockAnimationChange => false;

        public event Action UpdateStateEvent;
        public BossSkill2State(Action bossSkill2State)
        {
            UpdateStateEvent += bossSkill2State;
        }
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
}
