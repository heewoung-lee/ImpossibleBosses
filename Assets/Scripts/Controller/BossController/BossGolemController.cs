using BaseStates;
using System.Collections.Generic;
using UnityEngine;

public enum GolemAttackType
{
    NormalAttack,
    Skill1,
    Skill2,
}
public class BossGolemController : BossController
{
  
    private const float ATTACK_PRE_FRAME = 0.35f;
    private const float SKILL1_PRE_FRAME = 0.8f;
    private const float SKILL2_PRE_FRAME = 0.3f;


    private Dictionary<IState, float> _attackTypeDict = new Dictionary<IState, float>();
    public override Dictionary<IState,float> AttackTypeDict => _attackTypeDict;


    private int[] _golem_Attacks = new int[2]
    {
        Animator.StringToHash("Golem_Attack1"),
        Animator.StringToHash("Golem_Attack2")
    };

    protected override int Hash_Idle => Enemy_Anim_Hash.Golem_Idle;
    protected override int Hash_Move => Enemy_Anim_Hash.Golem_Walk;
    protected override int Hash_Attack => _golem_Attacks[UnityEngine.Random.Range(0, 2)];
    protected override int Hash_Die => Enemy_Anim_Hash.Golem_Dead;

    private int hash_golem_Skill1 = Enemy_Anim_Hash.Golem_Attacked;
    private int hash_golem_Skill2 = Enemy_Anim_Hash.Golem_Skill;

    public override AttackState Base_Attackstate => _base_Attackstate;
    public override IDleState Base_IDleState => _base_IDleState;
    public override DieState Base_DieState => _base_DieState;
    public override MoveState Base_MoveState => _base_MoveState;
    public BossSkill1State BossSkill1State => _bossSkill1State;
    public BossSkill2State BossSkill2State => _bossSkill2State;

    private AttackState _base_Attackstate;
    private IDleState _base_IDleState;
    private DieState _base_DieState;
    private MoveState _base_MoveState;
    private BossSkill1State _bossSkill1State;
    private BossSkill2State _bossSkill2State;


    protected override void AwakeInit()
    {
        _base_Attackstate = new AttackState(UpdateAttack);
        _base_MoveState = new MoveState(UpdateMove);
        _base_DieState = new DieState(UpdateDie);
        _base_IDleState = new IDleState(UpdateIdle);

        _bossSkill1State = new BossSkill1State(UpdateAttack);
        _bossSkill2State = new BossSkill2State(UpdateAttack);

        _attackTypeDict.Add(_base_Attackstate, Transition_Attack);
        _attackTypeDict.Add(_bossSkill1State, Transition_Attack);
        _attackTypeDict.Add(_bossSkill2State, Transition_Attack);
    }
    public override void UpdateAttack()
    {
        if (CurrentStateType == Base_DieState)
            return;

        if (CurrentStateType != Base_Attackstate)
        {
            CurrentStateType = Base_Attackstate;
        }
    }

    public override void UpdateIdle()
    {
        if (CurrentStateType == Base_DieState)
            return;

        if (CurrentStateType != Base_IDleState)
        {
            CurrentStateType = Base_IDleState;
        }
    }

    public override void UpdateMove()
    {
        if (CurrentStateType == Base_DieState)
            return;

        if (CurrentStateType != Base_MoveState)
        {
            CurrentStateType = Base_MoveState;
        }
    }

    public override void UpdateDie()
    {
        if (CurrentStateType == Base_DieState)
        {
            CurrentStateType = Base_DieState;
        }
    }

    protected override void AddInitalizeStateDice()
    {
        StateAnimDict.RegisterState(Base_Attackstate, () => RunAnimation(Hash_Attack, Transition_Attack));
        StateAnimDict.RegisterState(_bossSkill1State, () => RunAnimation(hash_golem_Skill1,Transition_Attack));
        StateAnimDict.RegisterState(_bossSkill2State, () => RunAnimation(hash_golem_Skill2,Transition_Attack));
    }

    protected override void StartInit()
    {
    }
}