using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : Action
{
    private BossGolemController _controller;
    private float _elapsedTime = 0f;
    private float _animLength = 0f;
    private float _charging = 0f;
    public SharedProjector _attackIndicator = null;
    private bool _isAttackReady = false;
    private List<Vector3> _attackRangeParticlePos;
    private BossStats _stats;

    public int radius_Step = 0;
    public int Angle_Step = 0;

    public override void OnStart()
    {
        base.OnStart();
        _controller = Owner.GetComponent<BossGolemController>();
        _controller.UpdateAttack();
        _stats = _controller.GetComponent<BossStats>();
        _animLength = Utill.GetAnimationLength("Anim_Attack1", _controller.Anim);
        _attackIndicator.Value = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<Indicator_Controller>();
        _attackIndicator.Value.SetValue(_stats.ViewDistance, _stats.ViewAngle);
        _attackIndicator.Value.transform.SetParent(_controller.transform, false);
        _attackIndicator.Value.GetComponent<Poolable>().WorldPositionStays = false;

        _attackRangeParticlePos = TargetInSight.GeneratePositionsInSector(_controller.transform,
               _controller.GetComponent<IAttackRange>().ViewAngle,
               _controller.GetComponent<IAttackRange>().ViewDistance,
               Angle_Step, radius_Step);
    }


    public override TaskStatus OnUpdate()
    {
        _elapsedTime += Time.deltaTime * _controller.Anim.speed;
        _charging = Mathf.Clamp01(_charging += Time.deltaTime * 0.45f);

        _attackIndicator.Value.FillProgress = _charging;
        _attackIndicator.Value.UpdateProjectors();

        _isAttackReady = _controller.SetAnimationSpeed(_elapsedTime, _animLength, _controller.Base_Attackstate);
        if (_isAttackReady && _charging >= 1)
        {
            _controller.Anim.speed = 1;

            if (_attackIndicator.Value.gameObject.activeSelf)
            {
                foreach (Vector3 pos in _attackRangeParticlePos)
                {
                    Managers.VFX_Manager.GenerateParticle("Paticle/AttackEffect/Dust_Paticle", pos, 1f);
                }
                Managers.ResourceManager.DestroyObject(_attackIndicator.Value.gameObject);
                TargetInSight.AttackTargetInSector(_stats);
            }
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
        _charging = 0f;
        _isAttackReady = false;
        _attackRangeParticlePos = null;
    }
}
