using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BossAttack : BehaviorDesigner.Runtime.Tasks.Action
{

    private BossGolemController _controller;
    private float _elapsedTime = 0f;
    private float _animLength = 0f;
    private List<Vector3> _attackRangeParticlePos;
    private BossStats _stats;
    private bool _hasSpawnedParticles;

    [SerializeField] private SharedProjector _attack_indicator;
    private Indicator_Controller _indicator_controller;

    private Action<float> _animationSpeedChanged;
    public event Action<float> AnimationSpeedChanged
    {
        add => UniqueEventRegister.AddSingleEvent(ref _animationSpeedChanged, value);
        remove => UniqueEventRegister.RemovedEvent(ref _animationSpeedChanged, value);
    }

    public int radius_Step = 0;
    public int Angle_Step = 0;

    public override void OnStart()
    {
        base.OnStart();
        ChechedBossAttackField();
        SpawnAttackIndicator();
        CalculateBossAttackRange();


        void ChechedBossAttackField()
        {
            _controller = Owner.GetComponent<BossGolemController>();
            _controller.UpdateAttack();
            _stats = _controller.GetComponent<BossStats>();
            _animLength = Utill.GetAnimationLength("Anim_Attack1", _controller.Anim);
            _hasSpawnedParticles = false;
        }
        void SpawnAttackIndicator()
        {
            _indicator_controller = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<Indicator_Controller>();
            _attack_indicator.Value = _indicator_controller;
            _indicator_controller = Managers.RelayManager.SpawnNetworkOBJ(_indicator_controller.gameObject).GetComponent<Indicator_Controller>();
            _indicator_controller.SetValue(_stats.ViewDistance, _stats.ViewAngle, _controller.transform, IndicatorDoneEvent);
            void IndicatorDoneEvent()
            {
                if (_hasSpawnedParticles) return;
                string dustPath = "Prefabs/Paticle/AttackEffect/Dust_Paticle";

                SpawnParamBase param = SpawnParamBase.Create(argFloat:1f);
                Managers.RelayManager.NGO_RPC_Caller.SpawnObjectToLocal(_attackRangeParticlePos, dustPath, param);

                //foreach (var pos in _attackRangeParticlePos)
                //{
                //    Managers.VFX_Manager.GenerateParticle("Prefabs/Paticle/AttackEffect/Dust_Paticle", pos, 1f);
                //}




                TargetInSight.AttackTargetInSector(_stats);
                _hasSpawnedParticles = true;
            }
        }
        void CalculateBossAttackRange()
        {
            _attackRangeParticlePos = TargetInSight.GeneratePositionsInSector(_controller.transform,
            _controller.GetComponent<IAttackRange>().ViewAngle,
            _controller.GetComponent<IAttackRange>().ViewDistance,
            Angle_Step, radius_Step);
        }
    }


    public override TaskStatus OnUpdate()
    {
        float elaspedTime = UpdateElapsedTime();
        UpdateAnimationSpeed(elaspedTime);

        return _elapsedTime >= _animLength ? TaskStatus.Success: TaskStatus.Running;

        float UpdateElapsedTime()
        {
            _elapsedTime += Time.deltaTime * _controller.Anim.speed;
            return _elapsedTime;
           
        }
        void UpdateAnimationSpeed(float elapsedTime)
        {
            _controller.SetAnimationSpeed(elapsedTime, _animLength, _controller.Base_Attackstate, out float animSpeed);
            _animationSpeedChanged?.Invoke(animSpeed);
            if (_hasSpawnedParticles)
            {
                _controller.Anim.speed = 1;
                _animationSpeedChanged?.Invoke(_controller.Anim.speed);
            }
        }
    }


    public override void OnEnd()
    {
        base.OnEnd();
        _elapsedTime = 0f;
        _attackRangeParticlePos = null;
        _hasSpawnedParticles = false;
    }
}
