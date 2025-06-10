using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBossHpBelowCondition : Conditional
{
    public int HP_Percent;

    BaseStats stat;
    public override void OnStart()
    {
        base.OnStart();
        stat = Owner.GetComponent<BaseStats>();
    }
    public override TaskStatus OnUpdate()
    {
        if(stat.Hp <= stat.MaxHp/100*HP_Percent)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
}
