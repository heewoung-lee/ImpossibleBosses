using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BossController : BaseController
{
    public int Tick = 0;

    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Boss;
    public abstract Dictionary<IState, float> AttackPreFrameDict { get; }

    protected override void AwakeInit()
    {
    }
    public bool TryGetAnimationSpeed(float elapsedTime,float animLength, IState attackType, out float animSpeed, float animStopThreshold,float startAnimSpeed = 1f)
    {
        animSpeed = 0f;

        if(TryGetAttackTypePreTime(attackType,out float preTime) is false)
        {
            return false;
        }
        return TryGetAnimationSpeed(elapsedTime,animLength,preTime,out animSpeed, animStopThreshold,startAnimSpeed);
    }
    public bool TryGetAnimationSpeed(float elapsedTime, float animLength, float preTime, out float animSpeed, float animStopThreshold, float startAnimSpeed = 1f)
    {
        animSpeed = 0f;
        startAnimSpeed = Mathf.Clamp01(startAnimSpeed);
        animSpeed = Mathf.Lerp(startAnimSpeed, 0f, elapsedTime / (animLength * preTime));
        //Debug.Log($"∆Ω{Tick++} ¿¸≈∏¿”{preTime} æ÷¥‘∑©Ω∫{animLength} Ω«¡¶æ÷¥‘Ω∫««µÂ{Anim.speed} Ω∫∑πΩ∫»¶µÂ{animStopThreshold}{System.Environment.StackTrace}");
        Anim.speed = animSpeed;
        bool finished = animSpeed <= animStopThreshold;
        if (finished)
        {
            Anim.speed = 0f;
            animSpeed = 0f;
        }

        return finished;
    }


    public bool TryGetAttackTypePreTime(IState attackType,out float preTime)
    {
        if (AttackPreFrameDict.TryGetValue(attackType, out float preTimetoDict) == false)
        {
            Debug.LogError($"Attack type {attackType} not found in AttackPreFrameDict.");
            preTime = default;
            return false;
        }
        preTime = preTimetoDict;
        return true;
    }
}