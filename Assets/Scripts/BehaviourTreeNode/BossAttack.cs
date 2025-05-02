using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : BehaviorDesigner.Runtime.Tasks.Action
{

    private BossGolemController _controller;
    private float _elapsedTime = 0f;
    private float _animLength = 0f;
    private bool _ischargingDone = false;
    private bool _isAttackReady = false;
    private List<Vector3> _attackRangeParticlePos;
    private BossStats _stats;

    [SerializeField] private SharedProjector _attack_indicator;
    private Indicator_Controller _indicator_controller;

    private Action<float> _animationSpeedChanged;
    public event Action<float> AnimationSpeedChanged
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _animationSpeedChanged, value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _animationSpeedChanged, value);
        }
    }


    public int radius_Step = 0;
    public int Angle_Step = 0;

    public override void OnStart()
    {
        base.OnStart();
        _controller = Owner.GetComponent<BossGolemController>();
        _controller.UpdateAttack();
        _stats = _controller.GetComponent<BossStats>();


        _animLength = Utill.GetAnimationLength("Anim_Attack1", _controller.Anim);
        _indicator_controller = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<Indicator_Controller>();
        _attack_indicator.Value = _indicator_controller;
        _indicator_controller = Managers.RelayManager.SpawnNetworkOBJ(_indicator_controller.gameObject).GetComponent<Indicator_Controller>();
        _indicator_controller.SetValue(_stats.ViewDistance, _stats.ViewAngle, _controller.transform, DoneCharging);

        _attackRangeParticlePos = TargetInSight.GeneratePositionsInSector(_controller.transform,
               _controller.GetComponent<IAttackRange>().ViewAngle,
               _controller.GetComponent<IAttackRange>().ViewDistance,
               Angle_Step, radius_Step);


        void DoneCharging()
        {
            _ischargingDone = true;
        }
    }


    public override TaskStatus OnUpdate()
    {
        _elapsedTime += Time.deltaTime * _controller.Anim.speed;
        _isAttackReady = _controller.SetAnimationSpeed(_elapsedTime, _animLength, _controller.Base_Attackstate, out float animSpeed);
        _animationSpeedChanged?.Invoke(animSpeed);
        if (_isAttackReady == true && _ischargingDone == true)
        {
            _controller.Anim.speed = 1;
            _animationSpeedChanged?.Invoke(_controller.Anim.speed);

            foreach (Vector3 pos in _attackRangeParticlePos)
            {
                Managers.VFX_Manager.GenerateParticle("Prefabs/Paticle/AttackEffect/Dust_Paticle", pos, 1f);
            }
            //TargetInSight.AttackTargetInSector(_stats);
        }
        if (_elapsedTime >= _animLength)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }


    public override void OnEnd()
    {
        base.OnEnd();
        _elapsedTime = 0f;
        _ischargingDone = false;
        _isAttackReady = false;
        _attackRangeParticlePos = null;
    }
}
