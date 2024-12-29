using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DTT.AreaOfEffectRegions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BossSkill2 : Action
{
    private BossGolemController _controller;

    public SharedInt Damage;
    public float Attack_Range = 0f;
    public int Radius_Step = 0;
    public int Angle_Step = 0;

    public SharedProjector _attackIndicator = null;
    private float _elapsedTime = 0f;
    private float _charging = 0f;
    private float _animLength = 0f;
    private bool _isAttackReady = false;
    private List<Vector3> _attackRangeCirclePos;
    private BossStats _stats;


    public override void OnStart()
    {
        base.OnStart();
        _controller = Owner.GetComponent<BossGolemController>();
        _stats = _controller.GetComponent<BossStats>();
        _animLength = Utill.GetAnimationLength("Anim_Attack_AoE", _controller.Anim);
        _attackIndicator.Value = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<Indicator_Controller>();
        if (Attack_Range <= 0)
        {
            _controller.TryGetComponent(out BossStats stats);
            Attack_Range = stats.ViewDistance;
        }
        _attackIndicator.Value.SetValue(Attack_Range, 360);
        _attackIndicator.Value.transform.SetParent(_controller.transform, false);
        _attackIndicator.Value.GetComponent<Poolable>().WorldPositionStays = false;


        _attackRangeCirclePos = TargetInSight.GeneratePositionsInCircle(_controller.transform,
            Attack_Range,
             Radius_Step, Angle_Step);


        _controller.CurrentStateType = _controller.BossSkill2State;
    }

    public override TaskStatus OnUpdate()
    {

        _elapsedTime += Time.deltaTime * _controller.Anim.speed;
        _charging = Mathf.Clamp01(_charging += Time.deltaTime * 0.45f);

        _attackIndicator.Value.FillProgress = _charging;
        _attackIndicator.Value.UpdateProjectors();

        _isAttackReady = _controller.SetAnimationSpeed(_elapsedTime, _animLength, _controller.BossSkill2State);
        if (_isAttackReady && _charging >= 1)
        {
            _controller.Anim.speed = 1;

            if (_attackIndicator.Value.gameObject.activeSelf)
            {
                foreach (Vector3 pos in _attackRangeCirclePos)
                {
                    Managers.VFX_Manager.GenerateParticle("Paticle/AttackEffect/Dust_Paticle", pos, 1f);
                }
                Managers.ResourceManager.DestroyObject(_attackIndicator.Value.gameObject);
                TargetInSight.AttackTargetInCircle(_stats, Attack_Range,Damage.Value);
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
        _animLength = 0f;
        _isAttackReady = false;
        _attackRangeCirclePos = null;
    }
}