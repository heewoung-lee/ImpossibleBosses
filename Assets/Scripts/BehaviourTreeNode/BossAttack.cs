using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BossAttack : BehaviorDesigner.Runtime.Tasks.Action
{
    private readonly float _attackDurationTime = 2f;

    private BossGolemController _controller;
    private BossGolemNetworkController _networkController;
    private float _elapsedTime = 0f;
    private float _animLength = 0f;
    private List<Vector3> _attackRangeParticlePos;
    private BossStats _stats;
    private bool _hasSpawnedParticles;

    private NGO_Indicator_Controller _indicator_controller;
    
    public SharedProjector _attackIndicator;
    public int Radius_Step = 0;
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
            _networkController = Owner.GetComponent<BossGolemNetworkController>();
            _hasSpawnedParticles = false;
        }
        void SpawnAttackIndicator()
        {
            _indicator_controller = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<NGO_Indicator_Controller>();
            _attackIndicator.Value = _indicator_controller;
            _indicator_controller = Managers.RelayManager.SpawnNetworkOBJ(_indicator_controller.gameObject).GetComponent<NGO_Indicator_Controller>();
            _indicator_controller.SetValue(_stats.ViewDistance, _stats.ViewAngle, _controller.transform, _attackDurationTime,IndicatorDoneEvent);
            void IndicatorDoneEvent()
            {
                if (_hasSpawnedParticles) return;
                string dustPath = "Prefabs/Paticle/AttackEffect/Dust_Paticle";
                SpawnParamBase param = SpawnParamBase.Create(argFloat:1f);
                Managers.RelayManager.NGO_RPC_Caller.SpawnNonNetworkObject(_attackRangeParticlePos, dustPath, param);
                #region 5.6�� ��ƼŬ ������� ����
                //foreach (var pos in _attackRangeParticlePos)
                //{
                //    Managers.VFX_Manager.GenerateParticle("Prefabs/Paticle/AttackEffect/Dust_Paticle", pos, 1f);
                //} 5.6�� Update ���� ��ƼŬ���� ��Ʈ��ũ ���� + ��Ʈ��ũ ������Ʈ Ǯ������ ����ȭ�� �ߴµ�
                // ���� ������Ʈ Ǯ���� ��Ʈ��ũ�� �ϴٺ���, ��Ʈ��ũ�� �����ϰ� �ɸ�
                // �ذ������� RPC_Caller���� ������ ������Ʈ ���,��ġ,���� �Ķ���͸� ������
                // RPC_Caller�� ISpawnBehavior�������̽��� ��ӹ��� ������Ʈ�� �����ϰԲ� ����
                #endregion
                TargetInSight.AttackTargetInSector(_stats);
                _hasSpawnedParticles = true;
            }
        }
        void CalculateBossAttackRange()
        {
            _attackRangeParticlePos = TargetInSight.GeneratePositionsInSector(_controller.transform,
            _controller.GetComponent<IAttackRange>().ViewAngle,
            _controller.GetComponent<IAttackRange>().ViewDistance,
            Angle_Step, Radius_Step);
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
            _networkController.AnimSpeed = animSpeed;
            if (_hasSpawnedParticles)
            {
                _controller.Anim.speed = 1;
                _networkController.AnimSpeed = _controller.Anim.speed;
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
