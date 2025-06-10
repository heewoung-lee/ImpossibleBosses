using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttackProbability : Conditional
{
    public int successRate = 0;

    public override void OnStart()
    {
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        if (Random.Range(0, 100) < successRate)
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
