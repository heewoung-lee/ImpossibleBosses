using System.Collections.Generic;
using Controller.ControllerStats;
using NetWork.Boss_NGO;
using UnityEngine;
using Util;

namespace Controller
{
    public abstract class BossController : BaseController
    {
        public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Boss;
        public abstract Dictionary<IState, float> AttackStopTimingRatioDict { get; }

        protected override void AwakeInit()
        {
        }

        public bool TryGetAnimationSpeed(double elapsedTime, out float animSpeed, CurrentAnimInfo animinfo,bool isCheckattackIndicatorFinish)
        {
            animSpeed = 0f;
            float startAnimSpeed =animinfo.StartAnimationSpeed;
            animSpeed = Mathf.Lerp(startAnimSpeed, 0f, (float)(elapsedTime / (animinfo.AnimLength * animinfo.DecelerationRatio)));
            Anim.speed = animSpeed;
            bool finished = animSpeed <= animinfo.AnimStopThreshold&& isCheckattackIndicatorFinish == true;
            if (finished)
            {
                animSpeed = startAnimSpeed;
                Anim.speed = animSpeed;
            }
            return finished;
        }



        public bool TryGetAttackTypePreTime(IState attackType,out float preTime)
        {
            if (AttackStopTimingRatioDict.TryGetValue(attackType, out float preTimetoDict) == false)
            {
                Debug.LogError($"Attack type {attackType} not found in AttackPreFrameDict.");
                preTime = default;
                return false;
            }
            preTime = preTimetoDict;
            return true;
        }
    }
}