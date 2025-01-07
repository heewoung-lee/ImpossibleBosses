using BaseStates;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public abstract class BaseController : MonoBehaviour
{
    public abstract Define.WorldObject WorldobjectType { get; protected set; }
    private IState _currentStateType;
    private const float DEFALUT_Transition_Idle = 0.3f;
    private const float DEFALUT_Transition_Move = 0.15f;
    private const float DEFALUT_Transition_Attack = 0.3f;
    private const float DEFALUT_Transition_Die = 0.3f;

    private Animator _anim;
    private float _transition_Idle = DEFALUT_Transition_Idle;
    private float _transition_Move = DEFALUT_Transition_Move;
    private float _transition_Attack = DEFALUT_Transition_Attack;
    private float _transition_Die = DEFALUT_Transition_Die;
    private int _animLayer = 0;

    private StateAnimationDict _stateAnimDict = new StateAnimationDict();//스테이터스가 바뀌면 애니메이션을 호출하는 딕셔너리
    public StateAnimationDict StateAnimDict => _stateAnimDict;

    protected abstract int Hash_Idle { get; }
    protected abstract int Hash_Move { get; }
    protected abstract int Hash_Attack { get; }
    protected abstract int Hash_Die { get; }

    public abstract AttackState Base_Attackstate { get; }
    public abstract IDleState Base_IDleState { get; }
    public abstract DieState Base_DieState { get; }
    public abstract MoveState Base_MoveState { get; }

    public abstract void UpdateAttack();
    public abstract void UpdateIdle();
    public abstract void UpdateMove();
    public abstract void UpdateDie();


    public Animator Anim { get => _anim; protected set => _anim = value; }
    public float Transition_Idle { get => _transition_Idle; protected set => _transition_Idle = value; }
    public float Transition_Move { get => _transition_Move; protected set => _transition_Move = value; }
    public float Transition_Attack { get => _transition_Attack; protected set => _transition_Attack = value; }
    public float Transition_Die { get => _transition_Die; protected set => _transition_Die = value; }
    public int AnimLayer { get => _animLayer; protected set => _animLayer = value; }


    public IState CurrentStateType
    {
        get => _currentStateType;
        set
        {
            if (_currentStateType.lockAnimationChange == true)
                return;
            _currentStateType = value;
            _stateAnimDict.CallState(_currentStateType); // 현재 상태의 루프문 실행
        }
    }
    public void ChangeAnimIfCurrentIsDone(int currentAnimHash, IState changeState)
    {
        if (IsAnimationDone(currentAnimHash) == false)
            return;

        if (CurrentStateType.lockAnimationChange)
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
        _currentStateType = Base_IDleState; //기본 스테이터스 지정
    }
    private void Start()
    {
        StartInit();
    }

    protected abstract void AwakeInit();
    protected abstract void StartInit();

    public void SetDefalutTransition_Value()
    {
        _transition_Idle = DEFALUT_Transition_Idle;
        _transition_Move = DEFALUT_Transition_Move;
        _transition_Attack = DEFALUT_Transition_Attack;
        _transition_Die = DEFALUT_Transition_Die;
    }

    protected abstract void AddInitalizeStateDict();

    private void InitailizeStateDict()
    {
        _stateAnimDict.RegisterState(Base_Attackstate, () => RunAnimation(Hash_Attack, Transition_Attack));
        _stateAnimDict.RegisterState(Base_DieState, () => RunAnimation(Hash_Die, Transition_Die));
        _stateAnimDict.RegisterState(Base_IDleState, () => RunAnimation(Hash_Idle, Transition_Idle));
        _stateAnimDict.RegisterState(Base_MoveState, () => RunAnimation(Hash_Move, Transition_Move));
        AddInitalizeStateDict();
    }


    public void RunAnimation(int HashCode, float Transition_State)
    {
        if (HashCode == 0)
            return;

        _anim.CrossFade(HashCode, Transition_State, AnimLayer, 0f);
    }
}
