using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossController<TAttackType> : BaseController where TAttackType : Enum
{
    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Boss;
    public abstract Dictionary<TAttackType,float> AttackTypeDict { get; }

    public abstract TAttackType AttackType { get; set; }

    protected BehaviorTree tree;

    protected override void AwakeInit()
    {
        tree = GetComponent<BehaviorTree>();
    }

    public void SetStateMove()
    {
        if (CurrentStateType == Base_DieState)
            return;

        if (CurrentStateType != Base_MoveState)
        {
            CurrentStateType = Base_MoveState;
        }
    }

    public void SetStateAttack()
    {
        if (CurrentStateType == Base_DieState)
            return;

        if (CurrentStateType != Base_Attackstate)
        {
            CurrentStateType = Base_Attackstate;
        }
    }
    public void SetStateIdle()
    {
        if (CurrentStateType == Base_DieState)
            return;

        if (CurrentStateType != Base_IDleState)
        {
            CurrentStateType = Base_IDleState;
        }
    }
    public void SetStateDie()
    {
        if (CurrentStateType == Base_DieState)
        {
            CurrentStateType = Base_DieState;
        }
    }

    public void SetTransition_Attack(float transitionValue)
    {
        Transition_Attack = transitionValue;
    }
    public bool SetAnimationSpeed(float elapsedTime, float animLength, TAttackType attackType,float startAnimSpeed = 1f)
    {
        startAnimSpeed = Mathf.Clamp01(startAnimSpeed);
        if (AttackTypeDict.TryGetValue(attackType, out float attackPreTime) == false)
        {
            Debug.LogError($"Attack type {attackType} not found in AttackType dictionary.");
            return false;
        }

        float attack_Pre_Time = attackPreTime;

        Anim.speed = Mathf.Lerp(startAnimSpeed, 0, elapsedTime / (animLength * attack_Pre_Time));
        if (Anim.speed <= 0.05f)
        {
            Anim.speed = 0;
            return true;
        }
        return false;
    }

    private void Update()
    {
    }
}