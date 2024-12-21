using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BossDead : Action
{
    BaseController _ownerController;
    Define.State _state;
    public SharedArcRegionProjector _projector;
    public override void OnStart()
    {
        base.OnStart();
        _ownerController = Owner.GetComponent<BaseController>();
        _state = _ownerController.State;
    }


    public override TaskStatus OnUpdate()
    {
        if (_state == Define.State.Die)
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
