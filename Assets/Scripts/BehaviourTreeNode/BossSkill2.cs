using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

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

    public SharedInt Damage;
    public float Attack_Range = 0f;
    public int Radius_Step = 0;
    public int Angle_Step = 0;

    [SerializeField] private SharedProjector _attackIndicator;
    private NGO_Indicator_Controller _indicator_controller;

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
            if (Attack_Range <= 0)
            {
                _controller.TryGetComponent(out BossStats stats);
                Attack_Range = stats.ViewDistance;
            }
            _hasSpawnedParticles = false;
        }
        void SpawnAttackIndicator()
        {
            _indicator_controller = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<NGO_Indicator_Controller>();
            _attackIndicator.Value = _indicator_controller;
            _attackIndicator.Value.GetComponent<Poolable>().WorldPositionStays = false;
            _indicator_controller = Managers.RelayManager.SpawnNetworkOBJ(_indicator_controller.gameObject).GetComponent<NGO_Indicator_Controller>();
            float _totalIndicatorDurationTime = _addAttackDurationTime + _animLength;
            _indicator_controller.SetValue(Attack_Range, 360, _controller.transform, _totalIndicatorDurationTime, IndicatorDoneEvent);
            OnBossGolemAnimationChanged(_bossGolemAnimationNetworkController, _controller.BossSkill2State);
            void IndicatorDoneEvent()
            {
                if (_hasSpawnedParticles == true) return;
                string dustPath = "Prefabs/Paticle/AttackEffect/Dust_Paticle_Big";
                SpawnParamBase param = SpawnParamBase.Create(argFloat: 1f);
                Managers.RelayManager.NGO_RPC_Caller.SpawnNonNetworkObject(_attackRangeCirclePos, dustPath, param);
                TargetInSight.AttackTargetInCircle(_stats, Attack_Range, Damage.Value);
                _hasSpawnedParticles = true;
            }
        }
        void CalculateBossAttackRange()
        {
            _attackRangeCirclePos = TargetInSight.GeneratePositionsInCircle(_controller.transform, Attack_Range, Radius_Step, Angle_Step);
        }
        void StartAnimationSpeedChanged()
        {
            if (_controller.TryGetAttackTypePreTime(_controller.BossSkill2State, out float decelerationRatio) is false)
                return;


            CurrentAnimInfo animinfo = new CurrentAnimInfo(_animLength, decelerationRatio, _attackAnimStopThreshold, _addAttackDurationTime, Managers.RelayManager.NetworkManagerEx.ServerTime.Time);
            _networkController.StartAnimChagnedRpc(animinfo, Managers.RelayManager.GetNetworkObject(_indicator_controller.gameObject));
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