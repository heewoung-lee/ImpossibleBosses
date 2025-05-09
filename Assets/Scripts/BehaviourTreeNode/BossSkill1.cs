using BaseStates;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSkill1 : Action
{
    public SharedInt Damage;

    private const float MAX_HEIGHT = 3f;
    private const float START_SKILL1_ANIM_SPEED = 0.8f;
    private const int SPAWN_BOSS_SKILL1_TICK = 200;
    
    private readonly string skill1_indicator_Path = "Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator";
    private readonly string skill1_stone_Path = "Prefabs/Enemy/Boss/AttackPattren/BossSkill1";
    private readonly float skill1_DurationTime = 1f;

    private BossGolemController _controller;
    private BossGolemNetworkController _networkController;
    private BossStats _stats;

    private float _elapsedTime = 0f;
    private int _tickCounter = 0;
    private float _animLength = 0f;

    private bool _isAttackReady = false;
    private Collider[] allTargets;


    public override void OnStart()
    {
        base.OnStart();
        ChechedField();
        void ChechedField()
        {
            _controller = Owner.GetComponent<BossGolemController>();
            _stats = _controller.GetComponent<BossStats>();
            _animLength = Utill.GetAnimationLength("Anim_Hit", _controller.Anim);
            allTargets = Physics.OverlapSphere(Owner.transform.position, float.MaxValue, _stats.TarGetLayer);
            _controller.CurrentStateType = _controller.BossSkill1State;
            _networkController = Owner.GetComponent<BossGolemNetworkController>();
        }
    }

    public override TaskStatus OnUpdate()
    {
        float elaspedTime = UpdateElapsedTime();
        _isAttackReady = UpdateAnimSpeed(elaspedTime);
        SpawnIndicator();
        return _isAttackReady == true ? TaskStatus.Success : TaskStatus.Running;
        
        float UpdateElapsedTime()
        {
            _elapsedTime += Time.deltaTime * _controller.Anim.speed;
            return _elapsedTime;
        }
        bool UpdateAnimSpeed(float elaspedTime)
        {
            bool isAttackReady =  _controller.SetAnimationSpeed(elaspedTime, _animLength, _controller.BossSkill1State, out float animSpeed, START_SKILL1_ANIM_SPEED);
            _networkController.AnimSpeed = animSpeed;
            return isAttackReady;
        }
        void SpawnIndicator()
        {
            _tickCounter++;
            if (_tickCounter >= SPAWN_BOSS_SKILL1_TICK)
            {
                _tickCounter = 0;
                foreach (Collider targetPlayer in allTargets)
                {
                    if (targetPlayer.TryGetComponent(out BaseStats targetBaseStats))
                    {
                        if (targetBaseStats.IsDead)
                            continue;
                    }
                    Vector3 targetPos = targetPlayer.transform.position;

                    SpawnParamBase skill1_indicator_param = SpawnParamBase.Create(argPosVector3: targetPos, argInteger: Damage.Value,argFloat: skill1_DurationTime);
                    Managers.RelayManager.NGO_RPC_Caller.SpawnLocalObject(targetPos, skill1_indicator_Path, skill1_indicator_param);

                    SpawnParamBase skill1_stone_param = SpawnParamBase.Create(argFloat: skill1_DurationTime);
                    Managers.RelayManager.NGO_RPC_Caller.SpawnLocalObject(targetPos, skill1_stone_Path, skill1_stone_param);
                    //5.6 ¼öÁ¤ SpawnProjector(targetPlayer);
                }
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _elapsedTime = 0;
        _controller.Anim.speed = 1f;
    }
}