using BehaviorDesigner.Runtime;
using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public struct CurrentAnimInfo : INetworkSerializable
{
    public float AnimLength;
    public float DecelerationRatio;
    public float AnimStopThreshold;
    public float IndicatorDuration;
    public float StartAnimationSpeed;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref AnimLength);
        serializer.SerializeValue(ref DecelerationRatio);
        serializer.SerializeValue(ref AnimStopThreshold);
        serializer.SerializeValue(ref IndicatorDuration);
        serializer.SerializeValue(ref StartAnimationSpeed);
    }

    public CurrentAnimInfo(float animLength, float decelerationRatio, float animStopThreshold,float duration,float startAnimSpeed = 1f)
    {
        AnimLength = animLength;
        DecelerationRatio = decelerationRatio;
        AnimStopThreshold = animStopThreshold;
        IndicatorDuration = duration;
        StartAnimationSpeed = startAnimSpeed;
    }
}


public class BossGolemNetworkController : NetworkBehaviourBase
{
    private BehaviorTree _bossBehaviourTree;
    private BossGolemController _bossController;
    private bool _finishedAttack = false;
    private Coroutine _animationCoroutine;
    private bool _finishedIndicatorDuration = false;

    public bool FinishAttack
    {
        get => _finishedAttack;
        private set => _finishedAttack = value;
    }
    protected override void AwakeInit()
    {
        _bossController = GetComponent<BossGolemController>();
        _bossBehaviourTree = GetComponent<BehaviorTree>();
    }
    protected override void StartInit()
    {
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.GameManagerEx.SetBossMonster(gameObject);
        if (IsHost == false)
        {
            InitBossOnClient();
        }
        void InitBossOnClient()
        {
            GetComponent<BossController>().enabled = false;
            GetComponent<BossGolemStats>().enabled = false;
            GetComponent<BehaviorTree>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartAnimChagnedRpc(CurrentAnimInfo animinfo)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        if (animinfo.IndicatorDuration < 0f)        // faster swing
        {
            float speedScale = animinfo.AnimLength / Mathf.Max(animinfo.AnimLength + animinfo.IndicatorDuration, 0.1f);
            animinfo.StartAnimationSpeed *= speedScale;
            animinfo.AnimStopThreshold *= speedScale;
        }

        _animationCoroutine = StartCoroutine(UpdateAnimCorutine(animinfo));
    }


    IEnumerator UpdateAnimCorutine(CurrentAnimInfo animinfo)
    {
        float elaspedTime = 0f;
        FinishAttack = false;

        double prevNetTime = NetworkManager.Singleton.ServerTime.Time;

        StartCoroutine(UpdateIndicatorDurationTime(animinfo.IndicatorDuration,animinfo.AnimLength));
        while (elaspedTime <= animinfo.AnimLength)
        {
            double currentNetTime = NetworkManager.Singleton.ServerTime.Time;
            float deltaTime = (float)(currentNetTime - prevNetTime);
            prevNetTime = currentNetTime;

            _bossController.TryGetAnimationSpeed(elaspedTime, out float animspeed, animinfo, _finishedIndicatorDuration);
            elaspedTime += deltaTime * animspeed;
            yield return null;
        }
        FinishAttack = true;
        _bossController.Anim.speed = 1;
    }


    IEnumerator UpdateIndicatorDurationTime(float indicatorAddduration,float animLength)
    {
        _finishedIndicatorDuration = false;
        float elapsedTime = 0f;
        while (elapsedTime <= indicatorAddduration+ animLength)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _finishedIndicatorDuration = true;
    }


}