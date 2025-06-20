using BaseStates;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSkill1 : Action, IBossAnimationChanged
{
    private readonly string skill1_indicator_Path = "Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator";
    private readonly string skill1_stone_Path = "Prefabs/Enemy/Boss/AttackPattren/BossSkill1";
    private readonly float skill1_DurationTime = 1f;
    private readonly float skill1_AnimStopThreshold = 0.02f;
    private readonly float skill1_StartAnimSpeed = 1f;

    private const float MAX_HEIGHT = 3f;
    private const float START_SKILL1_ANIM_SPEED = 0.8f;
    private const int SPAWN_BOSS_SKILL1_TICK = 20;
    
    private BossGolemController _controller;
    private BossGolemNetworkController _networkController;
    private BossStats _stats;
    private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;

    private int _tickCounter = 0;
    private float _animLength = 0f;

    private Collider[] _allTargets;

    public SharedInt Damage;

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
            CurrentAnimInfo animinfo = new CurrentAnimInfo(_animLength, decelerationRatio, skill1_AnimStopThreshold,skill1_DurationTime,Managers.RelayManager.NetworkManagerEx.ServerTime.Time, skill1_StartAnimSpeed);
            _networkController.StartAnimChagnedRpc(animinfo);
        }
    }

    public override TaskStatus OnUpdate()
    {
        SpawnIndicator();
        return _networkController.FinishAttack == true ? TaskStatus.Success : TaskStatus.Running;
        void SpawnIndicator()
        {
            _tickCounter++;
            if (_tickCounter >= SPAWN_BOSS_SKILL1_TICK)
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

                    SpawnParamBase skill1_indicator_param = SpawnParamBase.Create(argPosVector3: targetPos, argInteger: Damage.Value, argFloat: skill1_DurationTime);
                    Managers.RelayManager.NGO_RPC_Caller.SpawnLocalObject(targetPos, skill1_indicator_Path, skill1_indicator_param);

                    SpawnParamBase skill1_stone_param = SpawnParamBase.Create(argFloat: skill1_DurationTime);
                    Managers.RelayManager.NGO_RPC_Caller.SpawnLocalObject(targetPos, skill1_stone_Path, skill1_stone_param);
                    //5.6 ���� SpawnProjector(targetPlayer);
                }
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _controller.CurrentStateType = _controller.Base_IDleState;
    }
    public void OnBossGolemAnimationChanged(BossGolemAnimationNetworkController bossAnimController, IState state)
    {
        bossAnimController.SyncBossStateToClients(state);
    }
}