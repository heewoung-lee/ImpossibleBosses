using System;
using Controller.ControllerStats;

namespace Controller.BossState
{
    public class BossNormalAttackState : IState
    {
        public bool LockAnimationChange => false;

        public event Action UpdateStateEvent;
        public BossNormalAttackState(Action bossNormalAttackState)
        {
            UpdateStateEvent += bossNormalAttackState;
        }
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
}
