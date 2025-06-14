using Controller.ControllerStats.BaseStates;
using UnityEngine.AI;
using Util;

namespace Controller
{
    public class MonsterController : MoveableController
    {
        public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Monster;

        protected override int HashIdle => 0;

        protected override int HashMove => 0;

        protected override int HashAttack => 0;

        protected override int HashDie => 0;

        public override AttackState BaseAttackState => _baseAttackState;
        public override IDleState BaseIDleState => _baseIDleState;
        public override DieState BaseDieState => _baseDieState;
        public override MoveState BaseMoveState => _baseMoveState;

        private AttackState _baseAttackState;
        private IDleState _baseIDleState;
        private DieState _baseDieState;
        private MoveState _baseMoveState;
        private NavMeshAgent _agent;
    
        public override void UpdateAttack()
        {
        }

        public override void UpdateDie()
        {
        }

        public override void UpdateIdle()
        {
        }

        public override void UpdateMove()
        {
        }
    

        protected override void AwakeInit()
        {
            _baseAttackState = new AttackState(UpdateAttack);
            _baseMoveState = new MoveState(UpdateMove);
            _baseDieState = new DieState(UpdateDie);
            _baseIDleState = new IDleState(UpdateIdle);
        }
        protected override void StartInit()
        {


        }

        protected override void AddInitalizeStateDict()
        {
        }
    }
}
