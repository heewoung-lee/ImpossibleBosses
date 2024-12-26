using System;

namespace BaseStates
{
    public class AttackState : IState
    {
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