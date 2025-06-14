using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Controller.BossState;
using Controller.ControllerStats;
using GameManagers;
using NetWork;
using NetWork.Boss_NGO;
using NetWork.NGO.Interface;
using Stats.BaseStats;
using Stats.BossStats;
using UnityEngine;
using Util;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class BossSkill1 : Action, IBossAnimationChanged
    {
        private readonly string _skill1IndicatorPath = "Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator";
        private readonly string _skill1StonePath = "Prefabs/Enemy/Boss/AttackPattren/BossSkill1";
        private readonly float _skill1DurationTime = 1f;
        private readonly float _skill1AnimStopThreshold = 0.02f;
        private readonly float _skill1StartAnimSpeed = 1f;

        private const int SpawnBossSkill1Tick = 20;
    
        private BossGolemController _controller;
        private BossGolemNetworkController _networkController;
        private BossStats _stats;
        private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;

        private int _tickCounter = 0;
        private float _animLength = 0f;

        private Collider[] _allTargets;

        [SerializeField]private SharedInt _damage;

        public BossGolemAnimationNetworkController BossAnimNetworkController => _bossGolemAnimationNetworkController;

        public override void OnAwake()
        {
            base.OnAwake();
            ChechedField();
            void ChechedField()
            {
                _controller = Owner.GetComponent<BossGolemController>();
                _stats = _controller.GetComponent<BossStats>();
                _animLength = Utill.GetAnimationLength("Anim_Hit", _controller.Anim);
                _bossGolemAnimationNetworkController = Owner.GetComponent<BossGolemAnimationNetworkController>();
                _networkController = Owner.GetComponent<BossGolemNetworkController>();
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            StartAnimationSpeedChanged();
            void StartAnimationSpeedChanged()
            {
                if (_controller.TryGetAttackTypePreTime(_controller.BossSkill1State, out float decelerationRatio) is false)
                    return;

                _allTargets = Physics.OverlapSphere(Owner.transform.position, float.MaxValue, _stats.TarGetLayer);
                OnBossGolemAnimationChanged(BossAnimNetworkController, _controller.BossSkill1State);
                CurrentAnimInfo animInfo = new CurrentAnimInfo(_animLength, decelerationRatio, _skill1AnimStopThreshold,_skill1DurationTime,Managers.RelayManager.NetworkManagerEx.ServerTime.Time, _skill1StartAnimSpeed);
                _networkController.StartAnimChagnedRpc(animInfo);
            }
        }

        public override TaskStatus OnUpdate()
        {
            SpawnIndicator();
            return _networkController.FinishAttack == true ? TaskStatus.Success : TaskStatus.Running;
            void SpawnIndicator()
            {
                _tickCounter++;
                if (_tickCounter >= SpawnBossSkill1Tick)
                {
                    _tickCounter = 0;
                    foreach (Collider targetPlayer in _allTargets)
                    {
                        if (targetPlayer.TryGetComponent(out BaseStats targetBaseStats))
                        {
                            if (targetBaseStats.IsDead)
                                continue;
                        }
                        Vector3 targetPos = targetPlayer.transform.position;

                        SpawnParamBase skill1IndicatorParam = SpawnParamBase.Create(argPosVector3: targetPos, argInteger: _damage.Value, argFloat: _skill1DurationTime);
                        Managers.RelayManager.NgoRPCCaller.SpawnLocalObject(targetPos, _skill1IndicatorPath, skill1IndicatorParam);

                        SpawnParamBase skill1StoneParam = SpawnParamBase.Create(argFloat: _skill1DurationTime);
                        Managers.RelayManager.NgoRPCCaller.SpawnLocalObject(targetPos, _skill1StonePath, skill1StoneParam);
                        //5.6 수정 SpawnProjector(targetPlayer);
                    }
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _controller.CurrentStateType = _controller.BaseIDleState;
        }
        public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state)
        {
            bossAnimController.SyncBossStateToClients(state);
        }
    }
}