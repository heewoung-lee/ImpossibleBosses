using BaseStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MoveableController
{
    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Monster;

    protected override int Hash_Idle => 0;

    protected override int Hash_Move => 0;

    protected override int Hash_Attack => 0;

    protected override int Hash_Die => 0;

    public override AttackState Base_Attackstate => _base_Attackstate;
    public override IDleState Base_IDleState => _base_IDleState;
    public override DieState Base_DieState => _base_DieState;
    public override MoveState Base_MoveState => _base_MoveState;

    private AttackState _base_Attackstate;
    private IDleState _base_IDleState;
    private DieState _base_DieState;
    private MoveState _base_MoveState;
    NavMeshAgent _agent;
    
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
        _base_Attackstate = new AttackState(UpdateAttack);
        _base_MoveState = new MoveState(UpdateMove);
        _base_DieState = new DieState(UpdateDie);
        _base_IDleState = new IDleState(UpdateIdle);
    }
    protected override void StartInit()
    {


    }

    protected override void AddInitalizeStateDice()
    {
    }
}
