using System;

namespace BaseStates
{
    public class AttackState : IState
    {
        public bool lockAnimationChange => false;

        public event Action UpdateStateEvent;
        public AttackState(Action attackMethod) 
        {
            UpdateStateEvent += attackMethod;
        }
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
    public class DieState : IState
    {
        public bool lockAnimationChange => true;

        public DieState(Action dieMethod)
        {
            UpdateStateEvent += dieMethod;
        }

        public event Action UpdateStateEvent;
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
    public class IDleState : IState
    {
        public bool lockAnimationChange => false;

        public IDleState(Action iDleMethod)
        {
            UpdateStateEvent += iDleMethod;
        }

        public event Action UpdateStateEvent;
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
    public class MoveState : IState
    {
        public bool lockAnimationChange => false;

        public MoveState(Action moveMethod)
        {
            UpdateStateEvent += moveMethod;
        }

        public event Action UpdateStateEvent;
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }

}