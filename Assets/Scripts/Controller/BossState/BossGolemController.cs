using System.Collections.Generic;
using Controller.ControllerStats;
using Controller.ControllerStats.BaseStates;
using Data;
using UnityEngine;

namespace Controller.BossState
{
    public enum GolemAttackType
    {
        NormalAttack,
        Skill1,
        Skill2,
    }
    public class BossGolemController : BossController
    {

        private const float AttackPreFrame = 0.35f;
        private const float Skill1PreFrame = 0.6f;
        private const float Skill2PreFrame = 0.3f;
        private const float Skill1Transition = 0.1f;


        private Dictionary<IState, float> _attackStopTimingRatioDict = new Dictionary<IState, float>();
        public override Dictionary<IState, float> AttackStopTimingRatioDict => _attackStopTimingRatioDict;


        private int[] _golemAttacks = new int[2]
        {
            Animator.StringToHash("Golem_Attack1"),
            Animator.StringToHash("Golem_Attack2")
        };

        protected override int HashIdle => EnemyAnimHash.GolemIdle;
        protected override int HashMove => EnemyAnimHash.GolemWalk;
        protected override int HashAttack => _golemAttacks[UnityEngine.Random.Range(0, 2)];
        protected override int HashDie => EnemyAnimHash.GolemDead;

        private int _hashGolemSkill1 = EnemyAnimHash.GolemAttacked;
        private int _hashGolemSkill2 = EnemyAnimHash.GolemSkill;

        public override AttackState BaseAttackState => _baseAttackState;
        public override IDleState BaseIDleState => _baseIDleState;
        public override DieState BaseDieState => _baseDieState;
        public override MoveState BaseMoveState => _baseMoveState;
        public BossSkill1State BossSkill1State => _bossSkill1State;
        public BossSkill2State BossSkill2State => _bossSkill2State;


        private AttackState _baseAttackState;
        private IDleState _baseIDleState;
        private DieState _baseDieState;
        private MoveState _baseMoveState;
        private BossSkill1State _bossSkill1State;
        private BossSkill2State _bossSkill2State;

        protected override void AwakeInit()
        {
            _baseAttackState = new AttackState(UpdateAttack);
            _baseMoveState = new MoveState(UpdateMove);
            _baseDieState = new DieState(UpdateDie);
            _baseIDleState = new IDleState(UpdateIdle);

            _bossSkill1State = new BossSkill1State(UpdateAttack);
            _bossSkill2State = new BossSkill2State(UpdateAttack);

            _attackStopTimingRatioDict.Add(_baseAttackState, AttackPreFrame);
            _attackStopTimingRatioDict.Add(_bossSkill1State, Skill1PreFrame);
            _attackStopTimingRatioDict.Add(_bossSkill2State, Skill2PreFrame);
        }
        public override void UpdateAttack()
        {
            if (CurrentStateType == BaseDieState)
                return;

            CurrentStateType = BaseAttackState;
        }

        public override void UpdateIdle()
        {
            if (CurrentStateType != BaseIDleState)
            {
                CurrentStateType = BaseIDleState;
            }
        }

        public override void UpdateMove()
        {
            if(CurrentStateType != BaseMoveState)
            {
                CurrentStateType = BaseMoveState;
            }

        }
        private void Update()
        {
        }
        public override void UpdateDie()
        {
            if (CurrentStateType == BaseDieState)
            {
                CurrentStateType = BaseDieState;
            }
        }

        protected override void AddInitalizeStateDict()
        {
            StateAnimDict.RegisterState(_bossSkill1State, () => RunAnimation(_hashGolemSkill1, Skill1Transition));
            StateAnimDict.RegisterState(_bossSkill2State, () => RunAnimation(_hashGolemSkill2, TransitionAttack));
        }

        protected override void StartInit()
        {
        }
    }
}