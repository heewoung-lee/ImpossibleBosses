using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPhaseSkillUsed : Conditional
{

    public SharedBool _isPhaseSkillUsed;
    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        if(_isPhaseSkillUsed.Value == false)
        {
            _isPhaseSkillUsed.Value = true;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
}
