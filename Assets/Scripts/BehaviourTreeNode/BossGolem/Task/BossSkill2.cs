using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Controller.BossState;
using Controller.ControllerStats;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using NetWork;
using NetWork.Boss_NGO;
using NetWork.NGO.Interface;
using Stats.BossStats;
using UnityEngine;
using Util;
using VFX;
using Zenject;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class BossSkill2 : Action, IBossAnimationChanged
    {
        [Inject] private IInstantiate _instantiate;
        [Inject] private RelayManager _relayManager;

        private readonly float _addAttackDurationTime = 0f;
        private readonly float _attackAnimStopThreshold = 0.05f;

        private BossGolemController _controller;
        private BossGolemNetworkController _networkController;
        private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;

        private float _animLength = 0f;
        private List<Vector3> _attackRangeCirclePos;
        private BossStats _stats;
        private bool _hasSpawnedParticles;

        [SerializeField] private SharedInt _damage;
        [SerializeField] private float _attackRange;
        [SerializeField] private int _radiusStep;
        [SerializeField] private int _angleStep;

        [SerializeField] private SharedProjector _attackIndicator;
        private NgoIndicatorController _indicatorController;

        public BossGolemAnimationNetworkController BossAnimNetworkController => _bossGolemAnimationNetworkController;

        public override void OnAwake()
        {
            base.OnAwake();
            _controller = Owner.GetComponent<BossGolemController>();
            _stats = _controller.GetComponent<BossStats>();
            _networkController = Owner.GetComponent<BossGolemNetworkController>();
            _bossGolemAnimationNetworkController = Owner.GetComponent<BossGolemAnimationNetworkController>();
        }

        public override void OnStart()
        {
            base.OnStart();
            ChechedBossAttackField();
            SpawnAttackIndicator();
            CalculateBossAttackRange();
            StartAnimationSpeedChanged();

            void ChechedBossAttackField()
            {
                _animLength = Utill.GetAnimationLength("Anim_Attack_AoE", _controller.Anim);
                if (_attackRange <= 0)
                {
                    _controller.TryGetComponent(out BossStats stats);
                    _attackRange = stats.ViewDistance;
                }
                _hasSpawnedParticles = false;
            }
            void SpawnAttackIndicator()
            {
                _indicatorController = _instantiate.InstantiateByPath("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<NgoIndicatorController>();
                _attackIndicator.Value = _indicatorController;
                _attackIndicator.Value.GetComponent<Poolable>().WorldPositionStays = false;
                _indicatorController = _relayManager.SpawnNetworkObj(_indicatorController.gameObject).GetComponent<NgoIndicatorController>();
                float totalIndicatorDurationTime = _addAttackDurationTime + _animLength;
                _indicatorController.SetValue(_attackRange, 360, _controller.transform, totalIndicatorDurationTime, IndicatorDoneEvent);
                OnBossGolemAnimationChanged(_bossGolemAnimationNetworkController, _controller.BossSkill2State);
                void IndicatorDoneEvent()
                {
                    if (_hasSpawnedParticles == true) return;
                    string dustPath = "Prefabs/Paticle/AttackEffect/Dust_Paticle_Big";
                    SpawnParamBase param = SpawnParamBase.Create(argFloat: 1f);
                    _relayManager.NgoRPCCaller.SpawnNonNetworkObject(_attackRangeCirclePos, dustPath, param);
                    TargetInSight.AttackTargetInCircle(_stats, _attackRange, _damage.Value);
                    _hasSpawnedParticles = true;
                }
            }
            void CalculateBossAttackRange()
            {
                _attackRangeCirclePos = TargetInSight.GeneratePositionsInCircle(_controller.transform, _attackRange,_angleStep ,_radiusStep);
            }
            void StartAnimationSpeedChanged()
            {
                if (_controller.TryGetAttackTypePreTime(_controller.BossSkill2State, out float decelerationRatio) is false)
                    return;


                CurrentAnimInfo animInfo = new CurrentAnimInfo(_animLength, decelerationRatio, _attackAnimStopThreshold, _addAttackDurationTime, _relayManager.NetworkManagerEx.ServerTime.Time);
                _networkController.StartAnimChagnedRpc(animInfo, _relayManager.GetNetworkObject(_indicatorController.gameObject));
                //호스트가 pretime 뽑아서 모든 클라이언트 들에게 던져야함.

            }
        }

        public override TaskStatus OnUpdate()
        {
            return _networkController.FinishAttack == true ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _attackRangeCirclePos = null;
            _hasSpawnedParticles = false;
        }
        public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state)
        {
            bossAnimController.SyncBossStateToClients(state);
        }


    }
}