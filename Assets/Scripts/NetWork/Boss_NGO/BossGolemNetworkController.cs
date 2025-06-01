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

    public CurrentAnimInfo(float animLength, float decelerationRatio, float animStopThreshold,float indicatorDuration,double serverTime,float startAnimSpeed = 1f)
    {
        AnimLength = animLength;
        DecelerationRatio = decelerationRatio;
        AnimStopThreshold = animStopThreshold;
        IndicatorDuration = indicatorDuration;
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
        _bossController.Anim.speed = animinfo.StartAnimationSpeed;
        _animationCoroutine = StartCoroutine(UpdateAnimCorutine(animinfo));
    }


    IEnumerator UpdateAnimCorutine(CurrentAnimInfo animinfo)
    {

        double elaspedTime = 0f;
        FinishAttack = false;

        double nowTime = Managers.RelayManager.NetworkManagerEx.ServerTime.Time;

        
        //현재 서버가 간 시간 구하기
        double serverPreTime =  animinfo.ServerTime- nowTime;
        Debug.Log($"{serverPreTime}현재 호스트앞서간 시간 차");

        double totaldistance = animinfo.AnimLength * animinfo.DecelerationRatio;

        Debug.Log($"{animinfo.AnimLength}애니메이션 길이 {animinfo.DecelerationRatio}애니메이션 줄어드는 한계점");

        double remainingAnimTime = totaldistance - serverPreTime;
        Debug.Log($"{remainingAnimTime}클라이언트가 가야할 남은거리");//호스트는 0으로 나와야함

        //호스트는 1이 나와야함
        double catchAnimSpeed = Math.Clamp(totaldistance/ remainingAnimTime, _normalAnimSpeed, _maxAnimSpeed);
        Debug.Log($"{catchAnimSpeed}애니메이션 스피드");


        StartCoroutine(UpdateIndicatorDurationTime(animinfo.IndicatorDuration,animinfo.AnimLength, nowTime));
        while (elaspedTime <= animinfo.AnimLength)//경과시간을 빠르게 돌려야함 
        {
            double currentNetTime = NetworkManager.Singleton.ServerTime.Time;
            double deltaTime = (currentNetTime - nowTime);
            nowTime = currentNetTime;
            if (_bossController.TryGetAnimationSpeed(elaspedTime, out float animspeed, animinfo, _finishedIndicatorDuration) == false)
            {
                elaspedTime += deltaTime * animspeed * catchAnimSpeed;
            }
            else
            {
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