using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossController : BaseController
{
    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Boss;
    public abstract Dictionary<IState, float> AttackPreFrameDict { get; }

    protected override void AwakeInit()
    {
    }
    public bool SetAnimationSpeed(float elapsedTime,float animLength, IState attackType, out float animSpeed,float startAnimSpeed = 1f)
    {
        animSpeed = 0f;

        if (!AttackPreFrameDict.TryGetValue(attackType, out float preTime))
        {
            Debug.LogError($"Attack type {attackType} not found in AttackPreFrameDict.");
            return false;                  
        }

        startAnimSpeed = Mathf.Clamp01(startAnimSpeed);
        animSpeed = Mathf.Lerp(startAnimSpeed, 0f,elapsedTime / (animLength * preTime));
        Anim.speed = animSpeed;
        bool finished = animSpeed <= 0.05f;
        if (finished)
        {
            Anim.speed = 0f;
            animSpeed = 0f;
        }
        return finished;
    }
}