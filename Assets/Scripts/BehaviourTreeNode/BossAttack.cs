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
                #region 5.6일 파티클 스폰방식 수정
                //foreach (var pos in _attackRangeParticlePos)
                //{
                //    Managers.VFX_Manager.GenerateParticle("Prefabs/Paticle/AttackEffect/Dust_Paticle", pos, 1f);
                //} 5.6일 Update 이전 파티클들은 네트워크 스폰 + 네트워크 오브젝트 풀링으로 최적화를 했는데
                // 많은 오브젝트 풀링을 네트워크로 하다보니, 네트워크에 과부하가 걸림
                // 해결방반으로 RPC_Caller에게 스폰할 오브젝트 경로,위치,공통 파라미터만 보내고
                // RPC_Caller는 ISpawnBehavior인터페이스를 상속받은 오브젝트를 스폰하게끔 수정
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
        _attackRangeParticlePos = null;
        _hasSpawnedParticles = false;
    }
}
