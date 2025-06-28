using BehaviorDesigner.Runtime.Tasks;
using Controller.BossState;
using Controller.ControllerStats;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using NetWork.Boss_NGO;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class BossDead : Action, IBossAnimationChanged
    {
        
        [Inject] IDestroyObject _destroyer;
        [Inject] private RelayManager _relayManager;

        BossGolemController _controller;
        [SerializeField] private SharedProjector _projector;
        private BossGolemNetworkController _networkController;
        private Animator _anim;
        private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;
        private float _animLength;



        public BossGolemAnimationNetworkController BossAnimNetworkController => _bossGolemAnimationNetworkController;

        public override void OnAwake()
        {
            base.OnAwake();
            _controller = Owner.GetComponent<BossGolemController>();
            _networkController = Owner.GetComponent<BossGolemNetworkController>();
            _anim = Owner.GetComponent<Animator>();
            _bossGolemAnimationNetworkController = Owner.GetComponent <BossGolemAnimationNetworkController>();
            _animLength = Utill.GetAnimationLength("Anim_Dead", _controller.Anim);
        }

        public override void OnStart()
        {
            base.OnStart();
            OnBossGolemAnimationChanged(BossAnimNetworkController, _controller.BaseDieState);
            CurrentAnimInfo animInfo = new CurrentAnimInfo(_animLength, 0f, 0f, 0f, _relayManager.NetworkManagerEx.ServerTime.Time);
            _networkController.StartAnimChagnedRpc(animInfo);
        }


        public override TaskStatus OnUpdate()
        {
            if (_controller.CurrentStateType == _controller.BaseDieState)
            {
                if (_projector.Value != null && _projector.Value.GetComponent<NetworkObject>().IsSpawned)
                {
                    _destroyer.DestroyObject(_projector.Value.gameObject);
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
        public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state)
        {
            bossAnimController.SyncBossStateToClients(state);
        }
    }
}
