using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BossAttack : BehaviorDesigner.Runtime.Tasks.Action
{
    private readonly float _IndicatorAddDurationTime = 0f;
    private readonly float _attackAnimStopThreshold = 0.06f;

    private BossGolemController _controller;
    private BossGolemNetworkController _networkController;
    private float _animLength = 0f;
    private List<Vector3> _attackRangeParticlePos;
    private BossStats _stats;
    private bool _hasSpawnedParticles;

    private NGO_Indicator_Controller _indicator_controller;
    private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;

    public SharedProjector _attackIndicator;
    public int Radius_Step = 0;
    public int Angle_Step = 0;
    public override void OnAwake()
    {
        base.OnAwake();
        ChechedBossAttackField();
        void ChechedBossAttackField()
        {
            _controller = Owner.GetComponent<BossGolemController>();
            _stats = _controller.GetComponent<BossStats>();
            _animLength = Utill.GetAnimationLength("Anim_Attack1", _controller.Anim);
            _networkController = Owner.GetComponent<BossGolemNetworkController>();
            _bossGolemAnimationNetworkController = Owner.GetComponent<BossGolemAnimationNetworkController>();
        }
    }


    public override void OnStart()
    {
        base.OnStart();
        SpawnAttackIndicator();
        CalculateBossAttackRange();
        StartAnimationSpeedChanged();

        void SpawnAttackIndicator()
        {

            //_controller.UpdateAttack();
            _bossGolemAnimationNetworkController.SyncBossStateToClients(_controller.Base_Attackstate);
            _hasSpawnedParticles = false;

            _indicator_controller = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Attack_Indicator").GetComponent<NGO_Indicator_Controller>();
            _attackIndicator.Value = _indicator_controller;
            _indicator_controller = Managers.RelayManager.SpawnNetworkOBJ(_indicator_controller.gameObject).GetComponent<NGO_Indicator_Controller>();
            float totalIndicatorDurationTime = _IndicatorAddDurationTime + _animLength;
            _indicator_controller.SetValue(_stats.ViewDistance, _stats.ViewAngle, _controller.transform, totalIndicatorDurationTime, IndicatorDoneEvent);
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
        void StartAnimationSpeedChanged()
        {
            if (_controller.TryGetAttackTypePreTime(_controller.Base_Attackstate, out float decelerationRatio) is false)
                return;


            _controller.AttackStopTimingRatioDict.TryGetValue(_controller.Base_Attackstate,out float preframe);
            CurrentAnimInfo animinfo = new CurrentAnimInfo(_animLength, decelerationRatio, _attackAnimStopThreshold, _IndicatorAddDurationTime, preframe, Managers.RelayManager.NetworkManagerEx.ServerTime.Time);
            _networkController.StartAnimChagnedRpc(animinfo);
            //ȣ��Ʈ�� pretime �̾Ƽ� ��� Ŭ���̾�Ʈ �鿡�� ��������.
        }
    }


    public override TaskStatus OnUpdate()
    {
        return _networkController.FinishAttack == true ? TaskStatus.Success : TaskStatus.Running;
    }


    public override void OnEnd()
    {
        base.OnEnd();
        _attackRangeParticlePos = null;
        _hasSpawnedParticles = false;
    }
}
