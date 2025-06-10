using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Controller.BossState;
using Controller.ControllerStats;
using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class BossSkill2 : Action, IBossAnimationChanged
    {
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
        private NGO_Indicator_Controller _indicatorController;

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
                _indicatorController = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<NGO_Indicator_Controller>();
                _attackIndicator.Value = _indicatorController;
                _attackIndicator.Value.GetComponent<Poolable>().WorldPositionStays = false;
                _indicatorController = Managers.RelayManager.SpawnNetworkOBJ(_indicatorController.gameObject).GetComponent<NGO_Indicator_Controller>();
                float totalIndicatorDurationTime = _addAttackDurationTime + _animLength;
                _indicatorController.SetValue(_attackRange, 360, _controller.transform, totalIndicatorDurationTime, IndicatorDoneEvent);
                OnBossGolemAnimationChanged(_bossGolemAnimationNetworkController, _controller.BossSkill2State);
                void IndicatorDoneEvent()
                {
                    if (_hasSpawnedParticles == true) return;
                    string dustPath = "Prefabs/Paticle/AttackEffect/Dust_Paticle_Big";
                    SpawnParamBase param = SpawnParamBase.Create(argFloat: 1f);
                    Managers.RelayManager.NGO_RPC_Caller.SpawnNonNetworkObject(_attackRangeCirclePos, dustPath, param);
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


                CurrentAnimInfo animInfo = new CurrentAnimInfo(_animLength, decelerationRatio, _attackAnimStopThreshold, _addAttackDurationTime, Managers.RelayManager.NetworkManagerEx.ServerTime.Time);
                _networkController.StartAnimChagnedRpc(animInfo, Managers.RelayManager.GetNetworkObject(_indicatorController.gameObject));
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