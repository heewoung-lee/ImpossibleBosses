using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossController : BaseController
{
    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Boss;
    public abstract Dictionary<IState, float> AttackPreFrameDict { get; }

    protected BehaviorTree tree;

    private Action<float> _animationSpeedChanged;

    public event Action<float> AnimationSpeedChanged
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _animationSpeedChanged,value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _animationSpeedChanged, value);
        }
    }


    protected override void AwakeInit()
    {
        tree = GetComponent<BehaviorTree>();
    }
    public bool SetAnimationSpeed(float elapsedTime, float animLength, IState attackType,float startAnimSpeed = 1f)
    {
        startAnimSpeed = Mathf.Clamp01(startAnimSpeed);
        if (AttackPreFrameDict.TryGetValue(attackType, out float attackPreTime) == false)
        {
            Debug.LogError($"Attack type {attackType} not found in AttackType dictionary.");
            return false;
        }

        float attack_Pre_Time = attackPreTime;

        Anim.speed = Mathf.Lerp(startAnimSpeed, 0, elapsedTime / (animLength * attack_Pre_Time));
        _animationSpeedChanged?.Invoke(Anim.speed);
        if (Anim.speed <= 0.05f)
        {
            Anim.speed = 0;
            return true;
        }
        return false;
    }
}