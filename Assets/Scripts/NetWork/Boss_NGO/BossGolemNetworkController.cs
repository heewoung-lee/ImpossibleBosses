using BehaviorDesigner.Runtime;
using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public struct AnimChangeInfo : INetworkSerializable
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

    public AnimChangeInfo(float animLength, float decelerationRatio, float animStopThreshold,float duration,float startAnimSpeed = 1f)
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
    public void StartAnimChagnedRpc(AnimChangeInfo animinfo)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(UpdateAnimCorutine(animinfo));
    }


    IEnumerator UpdateAnimCorutine(AnimChangeInfo animinfo)
    {
        float elaspedTime = 0f;
        FinishAttack = false;
        StartCoroutine(UpdateIndicatorDurationTime(animinfo.IndicatorDuration));
        while (elaspedTime <= animinfo.AnimLength)
        {
            _bossController.TryGetAnimationSpeed(elaspedTime, out float animspeed, animinfo, _finishedIndicatorDuration);
            elaspedTime += Time.unscaledDeltaTime * animspeed;
            yield return null;
        }
        FinishAttack = true;
    }


    IEnumerator UpdateIndicatorDurationTime(float duration)
    {
        _finishedIndicatorDuration = false;
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _finishedIndicatorDuration = true;
    }


}