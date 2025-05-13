using BehaviorDesigner.Runtime.Tasks;
using Unity.Netcode;
using UnityEngine;

public class BossDead : Action
{
    BaseController _ownerController;
    [SerializeField]private SharedProjector _projector;
    private BossGolemNetworkController _networkController;

    public override void OnStart()
    {
        base.OnStart();
        _ownerController = Owner.GetComponent<BaseController>();
        _networkController = Owner.GetComponent<BossGolemNetworkController>();
    }


    public override TaskStatus OnUpdate()
    {
        if (_ownerController.CurrentStateType == _ownerController.Base_DieState)
        {
            //_networkController.AnimSpeed = _ownerController.Anim.speed;
            if (_projector.Value != null && _projector.Value.GetComponent<NetworkObject>().IsSpawned)
            {
                Managers.ResourceManager.DestroyObject(_projector.Value.gameObject);
                _projector.Value = null;
            }
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;

    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
