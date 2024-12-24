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
    

    void Update()
    {
        
    }

    protected override void AwakeInit()
    {
        State = Define.State.Idle;
    }
    protected override void StartInit()
    {
    }

}
