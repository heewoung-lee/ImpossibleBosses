using BaseStates;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static BehaviorDesigner.Runtime.BehaviorManager;

public class BossDead : Action
{
    BaseController _ownerController;
    public SharedArcRegionProjector _projector;

    public override void OnStart()
    {
        base.OnStart();
        _ownerController = Owner.GetComponent<BaseController>();
    }


    public override TaskStatus OnUpdate()
    {
        if (_ownerController.CurrentStateType == _ownerController.Base_DieState)
        {
            Managers.ResourceManager.DestroyObject(_projector.Value.gameObject);
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
