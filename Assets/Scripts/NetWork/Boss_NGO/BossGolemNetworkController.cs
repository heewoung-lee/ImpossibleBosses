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
    public double ServerTime;
    public float StartAnimationSpeed;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref AnimLength);
        serializer.SerializeValue(ref DecelerationRatio);
        serializer.SerializeValue(ref AnimStopThreshold);
        serializer.SerializeValue(ref IndicatorDuration);
        serializer.SerializeValue(ref ServerTime);
        serializer.SerializeValue(ref StartAnimationSpeed);
    }

    public CurrentAnimInfo(float animLength, float decelerationRatio, float animStopThreshold,float duration,double serverTime,float startAnimSpeed = 1f)
    {
        AnimLength = animLength;
        DecelerationRatio = decelerationRatio;
        AnimStopThreshold = animStopThreshold;
        IndicatorDuration = duration;
        ServerTime = serverTime;
        StartAnimationSpeed = startAnimSpeed;
    }
}


public class BossGolemNetworkController : NetworkBehaviourBase
{
    private readonly float _normalAnimSpeed = 1f;
    private readonly float _maxAnimSpeed = 3f;


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
            //GetComponent<BossGolemStats>().enabled = false;
            GetComponent<BehaviorTree>().enabled = false;
            //GetComponent<NavMeshAgent>().enabled = false;
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
        _bossController.Anim.speed = animinfo.StartAnimationSpeed;

        double nowTime = Managers.RelayManager.NetworkManagerEx.ServerTime.Time;
        //현재 서버가 간 시간 구하기
        double serverPreTime =  animinfo.ServerTime- nowTime;
        //서버를 따라잡으려면 어느정도 배속이 필요한지 구하기
        float remainingAnimTime = (float)(animinfo.AnimLength-animinfo.AnimStopThreshold - serverPreTime);
        float catchAnimSpeed = Mathf.Clamp(animinfo.AnimLength / remainingAnimTime, _normalAnimSpeed, _maxAnimSpeed);

        StartCoroutine(UpdateIndicatorDurationTime(animinfo.IndicatorDuration,animinfo.AnimLength, nowTime));
        while (elaspedTime <= animinfo.AnimLength)//경과시간을 빠르게 돌려야함 
        {
            double currentNetTime = NetworkManager.Singleton.ServerTime.Time;
            float deltaTime = (float)(currentNetTime - nowTime);
            nowTime = currentNetTime;
            _bossController.TryGetAnimationSpeed(elaspedTime, out float animspeed, animinfo, _finishedIndicatorDuration);

            

            if((elaspedTime / animinfo.AnimLength) <= (animinfo.AnimStopThreshold/ animinfo.AnimLength))
            {
                elaspedTime += deltaTime * animspeed * catchAnimSpeed;
                Debug.Log(animinfo.AnimStopThreshold / animinfo.AnimLength + "빨리 돌리기");
            }
            else
            {
                Debug.Log(animinfo.AnimStopThreshold / animinfo.AnimLength + "천천히 돌리기");
                elaspedTime += deltaTime * animspeed;
            }
            yield return null;
        }
        FinishAttack = true;
        _bossController.Anim.speed = 1;
    }


    IEnumerator UpdateIndicatorDurationTime(float indicatorAddduration,float animLength,double prevNetTime)
    {
        _finishedIndicatorDuration = false;
        float elapsedTime = 0f;
        while (elapsedTime <= indicatorAddduration+ animLength)
        {
            double currentNetTime = NetworkManager.Singleton.ServerTime.Time;
            float deltaTime = (float)(currentNetTime - prevNetTime);
            elapsedTime += deltaTime;
            yield return null;
        }
        _finishedIndicatorDuration = true;
    }


}