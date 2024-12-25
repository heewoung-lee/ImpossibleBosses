using System;

namespace BaseStates
{
    public class AttackState : IMoveableState
    {
        public AttackState(Action attackMethod) 
        {
            UpdateStateEvent += attackMethod;
        }
        public event Action UpdateStateEvent;
        public void UpdateState()
        {
            UpdateStateEvent?.Invoke();
        }
    }
    public class DieState : IMoveableState
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
    public class IDleState : IMoveableState
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
    public class MoveState : IMoveableState
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