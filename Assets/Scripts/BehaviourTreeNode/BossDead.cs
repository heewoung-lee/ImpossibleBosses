using BehaviorDesigner.Runtime.Tasks;
using Unity.Netcode;
using UnityEngine;

public class BossDead : Action
{
    BaseController _ownerController;
    [SerializeField]private SharedProjector _projector;
    private BossGolemNetworkController _networkController;
    private Animator _anim;

    public override void OnStart()
    {
        base.OnStart();
        _ownerController = Owner.GetComponent<BaseController>();
        _networkController = Owner.GetComponent<BossGolemNetworkController>();
        _anim = Owner.GetComponent<Animator>();
    }


    public override TaskStatus OnUpdate()
    {
        if (_ownerController.CurrentStateType == _ownerController.Base_DieState)
        {
            if (_projector.Value != null && _projector.Value.GetComponent<NetworkObject>().IsSpawned)
            {
                Managers.ResourceManager.DestroyObject(_projector.Value.gameObject);
                _projector.Value = null;
            }
            AnimatorStateInfo info = _anim.GetCurrentAnimatorStateInfo(0);
            bool isFinished = info.normalizedTime >= 1f && _anim.IsInTransition(0) == false;
            if (isFinished)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
        return TaskStatus.Failure;

    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
