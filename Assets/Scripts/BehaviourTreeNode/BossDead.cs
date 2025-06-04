using BehaviorDesigner.Runtime.Tasks;
using Unity.Netcode;
using UnityEngine;

public class BossDead : Action, IBossAnimationChanged
{
    BossGolemController _controller;
    [SerializeField] private SharedProjector _projector;
    private BossGolemNetworkController _networkController;
    private Animator _anim;
    private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;

    public BossGolemAnimationNetworkController BossAnimNetworkController => _bossGolemAnimationNetworkController;

    public override void OnAwake()
    {
        base.OnAwake();
        _controller = Owner.GetComponent<BossGolemController>();
        _networkController = Owner.GetComponent<BossGolemNetworkController>();
        _anim = Owner.GetComponent<Animator>();
        _bossGolemAnimationNetworkController = Owner.GetComponent <BossGolemAnimationNetworkController>();
    }

    public override void OnStart()
    {
        base.OnStart();
        OnBossGolemAnimationChanged(BossAnimNetworkController, _controller.Base_DieState);
    }


    public override TaskStatus OnUpdate()
    {
        if (_controller.CurrentStateType == _controller.Base_DieState)
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

    public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state)
    {
        bossAnimController.SyncBossStateToClients(state);
    }
}
