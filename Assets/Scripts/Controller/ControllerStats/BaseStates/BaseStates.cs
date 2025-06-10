using System;

namespace Controller.ControllerStats.BaseStates
{
    public class AttackState : IState
    {
        public bool LockAnimationChange => false;

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
        public bool LockAnimationChange => true;

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
        public bool LockAnimationChange => false;

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
        public bool LockAnimationChange => false;

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