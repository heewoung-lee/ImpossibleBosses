using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill2 : Action
{
    private readonly float _attackDurationTime = 3f;

    private BossGolemController _controller;
    private BossGolemNetworkController _networkController;

    public SharedInt Damage;
    public float Attack_Range = 0f;
    public int Radius_Step = 0;
    public int Angle_Step = 0;

    private float _elapsedTime = 0f;
    private float _animLength = 0f;
    private List<Vector3> _attackRangeCirclePos;
    private BossStats _stats;
    private bool _hasSpawnedParticles;

    [SerializeField] private SharedProjector _attackIndicator;
    private NGO_Indicator_Controller _indicator_controller;
    public override void OnStart()
    {
        base.OnStart();
        ChechedBossAttackField();
        SpawnAttackIndicator();
        CalculateBossAttackRange();


        void ChechedBossAttackField()
        {
            _controller = Owner.GetComponent<BossGolemController>();
            _stats = _controller.GetComponent<BossStats>();
            _animLength = Utill.GetAnimationLength("Anim_Attack_AoE", _controller.Anim);
            if (Attack_Range <= 0)
            {
                _controller.TryGetComponent(out BossStats stats);
                Attack_Range = stats.ViewDistance;
            }
            _networkController = Owner.GetComponent<BossGolemNetworkController>();
            _hasSpawnedParticles = false;
        }
        void SpawnAttackIndicator()
        {
            _indicator_controller = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<NGO_Indicator_Controller>();
            _attackIndicator.Value = _indicator_controller;
            _attackIndicator.Value.GetComponent<Poolable>().WorldPositionStays = false;
            _indicator_controller = Managers.RelayManager.SpawnNetworkOBJ(_indicator_controller.gameObject).GetComponent<NGO_Indicator_Controller>();
            _indicator_controller.SetValue(Attack_Range, 360, _controller.transform, _attackDurationTime, IndicatorDoneEvent);
            _controller.CurrentStateType = _controller.BossSkill2State;
            void IndicatorDoneEvent()
            {
                if (_hasSpawnedParticles) return;
                string dustPath = "Prefabs/Paticle/AttackEffect/Dust_Paticle_Big";
                SpawnParamBase param = SpawnParamBase.Create(argFloat: 1f);
                Managers.RelayManager.NGO_RPC_Caller.SpawnNonNetworkObject(_attackRangeCirclePos, dustPath, param);
                TargetInSight.AttackTargetInCircle(_stats, Attack_Range, Damage.Value);
                _hasSpawnedParticles = true;
            }
        }
        void CalculateBossAttackRange()
        {
            _attackRangeCirclePos = TargetInSight.GeneratePositionsInCircle(_controller.transform, Attack_Range,Radius_Step,Angle_Step);
        }
    }

    public override TaskStatus OnUpdate()
    {
        float elaspedTime = UpdateElapsedTime();
        UpdateAnimationSpeed(elaspedTime);

        return _elapsedTime >= _animLength ? TaskStatus.Success : TaskStatus.Running;

        float UpdateElapsedTime()
        {
            _elapsedTime += Time.deltaTime * _controller.Anim.speed;
            return _elapsedTime;
        }
        void UpdateAnimationSpeed(float elapsedTime)
        {
            _controller.TryGetAnimationSpeed(elapsedTime, _animLength, _controller.BossSkill2State, out float animSpeed);
            //    _networkController.AnimSpeed = animSpeed;
            //    if (_hasSpawnedParticles)
            //    {
            //        _controller.Anim.speed = 1;
            //        _networkController.AnimSpeed = _controller.Anim.speed;
            //    }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _elapsedTime = 0f;
        _attackRangeCirclePos = null;
        _hasSpawnedParticles = false;
    }
}