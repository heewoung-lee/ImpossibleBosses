using System;
using Controller.ControllerStats;

namespace Controller.BossState
{
    public class BossSkill1State : IState
    {
        public bool LockAnimationChange => false;

        public event Action UpdateStateEvent;
        public BossSkill1State(Action bossSkill1State)
        {
            UpdateStateEvent += bossSkill1State;
        }
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
}
