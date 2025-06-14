using Controller.ControllerStats;
using Controller.ControllerStats.BaseStates;
using UnityEngine;
using Util;

namespace Controller
{
    public abstract class BaseController : MonoBehaviour
    {
        public abstract Define.WorldObject WorldobjectType { get; protected set; }
        private IState _currentStateType;
        private const float DefalutTransitionIdle = 0.3f;
        private const float DefalutTransitionMove = 0.15f;
        private const float DefalutTransitionAttack = 0.3f;
        private const float DefalutTransitionDie = 0.3f;

        private Animator _anim;
        private float _transitionIdle = DefalutTransitionIdle;
        private float _transitionMove = DefalutTransitionMove;
        private float _transitionAttack = DefalutTransitionAttack;
        private float _transitionDie = DefalutTransitionDie;
        private int _animLayer;

        private GameObject _targetObject;
        public GameObject TargetObject { get => _targetObject; set => _targetObject = value; }


        private StateAnimationDict _stateAnimDict = new StateAnimationDict();//스테이터스가 바뀌면 애니메이션을 호출하는 딕셔너리
        public StateAnimationDict StateAnimDict => _stateAnimDict;

        protected abstract int HashIdle { get; }
        protected abstract int HashMove { get; }
        protected abstract int HashAttack { get; }
        protected abstract int HashDie { get; }

        public abstract AttackState BaseAttackState { get; }
        public abstract IDleState BaseIDleState { get; }
        public abstract DieState BaseDieState { get; }
        public abstract MoveState BaseMoveState { get; }

        public abstract void UpdateAttack();
        public abstract void UpdateIdle();
        public abstract void UpdateMove();
        public abstract void UpdateDie();


        public Animator Anim { get => _anim; protected set => _anim = value; }
        public float TransitionIdle { get => _transitionIdle; protected set => _transitionIdle = value; }
        public float TransitionMove { get => _transitionMove; protected set => _transitionMove = value; }
        public float TransitionAttack { get => _transitionAttack; protected set => _transitionAttack = value; }
        public float TransitionDie { get => _transitionDie; protected set => _transitionDie = value; }
        public int AnimLayer { get => _animLayer; protected set => _animLayer = value; }


        public IState CurrentStateType
        {
            get => _currentStateType;
            set
            {
                if (_currentStateType.LockAnimationChange == true)
                    return;
                _currentStateType = value;
                _stateAnimDict.CallState(_currentStateType); // 현재 상태의 루프문 실행
            }
        }


        public void ChangeAnimIfCurrentIsDone(int currentAnimHash, IState changeState)
        {
            if (IsAnimationDone(currentAnimHash) == false)
                return;

            if (CurrentStateType.LockAnimationChange)
            {
                _currentStateType = changeState;
                _stateAnimDict.CallState(_currentStateType);
            }
            else
            {
                CurrentStateType = changeState;
            }
        }
        public bool IsAnimationDone(int animHash)
        {
            AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(AnimLayer);
            //  스테이트가 정상 재생 중이며, 재생이 끝났는지 검사
            if (Anim.IsInTransition(AnimLayer) == false && stateInfo.shortNameHash == animHash && stateInfo.normalizedTime >= 1.0f)
                return true;

            return false;
        }
        private void Awake()
        {
            _anim = GetComponent<Animator>();
            AwakeInit();
            InitailizeStateDict(); //기본 스테이터스 초기화
            _currentStateType = BaseIDleState; //기본 스테이터스 지정
        }
        private void Start()
        {
            StartInit();
        }

        protected abstract void AwakeInit();
        protected abstract void StartInit();

        public void SetDefalutTransition_Value()
        {
            _transitionIdle = DefalutTransitionIdle;
            _transitionMove = DefalutTransitionMove;
            _transitionAttack = DefalutTransitionAttack;
            _transitionDie = DefalutTransitionDie;
        }

        protected abstract void AddInitalizeStateDict();

        private void InitailizeStateDict()
        {
            _stateAnimDict.RegisterState(BaseAttackState, () => RunAnimation(HashAttack, TransitionAttack));
            _stateAnimDict.RegisterState(BaseDieState, () => RunAnimation(HashDie, TransitionDie));
            _stateAnimDict.RegisterState(BaseIDleState, () => RunAnimation(HashIdle, TransitionIdle));
            _stateAnimDict.RegisterState(BaseMoveState, () => RunAnimation(HashMove, TransitionMove));
            AddInitalizeStateDict();
        }


        public void RunAnimation(int hashCode, float transitionState)
        {
            if (hashCode == 0)
                return;

            _anim.CrossFade(hashCode, transitionState, AnimLayer, 0f);
        }
    }
}
