namespace BaseStates
{
    public class AttackState : IMoveableState
    {
        public void UpdateState(MoveableController controller)
        {
            controller.UpdateAttack();
        }
    }
    public class DieState : IMoveableState
    {
        public void UpdateState(MoveableController controller)
        {
            controller.UpdateDie();
        }
    }
    public class IDleState : IMoveableState
    {
        public void UpdateState(MoveableController controller)
        {
            controller.UpdateIdle();
        }
    }
    public class MoveState : IMoveableState
    {
        public void UpdateState(MoveableController controller)
        {
           controller.UpdateMove();
        }
    }

}