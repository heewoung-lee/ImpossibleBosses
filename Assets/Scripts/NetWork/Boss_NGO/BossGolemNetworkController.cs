using BehaviorDesigner.Runtime;
using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public struct CurrentAnimInfo : INetworkSerializable
{
    public float AnimLength;
    public float DecelerationRatio;
    public float AnimStopThreshold;
    public float AddIndicatorDuration;
    public double ServerTime;
    public float StartAnimationSpeed;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref AnimLength);
        serializer.SerializeValue(ref DecelerationRatio);
        serializer.SerializeValue(ref AnimStopThreshold);
        serializer.SerializeValue(ref AddIndicatorDuration);
        serializer.SerializeValue(ref ServerTime);
        serializer.SerializeValue(ref StartAnimationSpeed);
    }

    public CurrentAnimInfo(float animLength, float decelerationRatio, float animStopThreshold,float AddindicatorDuration,double serverTime,float startAnimSpeed = 1f)
    {
        AnimLength = animLength;
        DecelerationRatio = decelerationRatio;
        AnimStopThreshold = animStopThreshold;
        AddIndicatorDuration = AddindicatorDuration;
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
            GetComponent<BehaviorTree>().enabled = false;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartAnimChagnedRpc(CurrentAnimInfo animinfo,NetworkObjectReference indicatorRef = default)
    {
        NGO_Indicator_Controller indicatorController = null;
        if (indicatorRef.Equals(default) == false)
        {
            if (indicatorRef.TryGet(out NetworkObject ngo))
            {
                indicatorController = ngo.GetComponent<NGO_Indicator_Controller>();
            }
        }
       

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        if (animinfo.AddIndicatorDuration < 0f)        // faster swing
        {
            float speedScale = animinfo.AnimLength / Mathf.Max(animinfo.AnimLength + animinfo.AddIndicatorDuration, 0.1f);
            animinfo.StartAnimationSpeed *= speedScale;
            animinfo.AnimStopThreshold *= speedScale;
        }
        _bossController.Anim.speed = animinfo.StartAnimationSpeed;
        _animationCoroutine = StartCoroutine(UpdateAnimCorutine(animinfo, indicatorController));
    }


    IEnumerator UpdateAnimCorutine(CurrentAnimInfo animinfo, NGO_Indicator_Controller indicatorCon = null)
    {

        double elaspedTime = 0f;
        FinishAttack = false;
        _finishedIndicatorDuration = false;
        double nowTime = Managers.RelayManager.NetworkManagerEx.ServerTime.Time;

        
        //���� ������ �� �ð�
        double serverPreTime =  animinfo.ServerTime- nowTime;

        //�ִϸ��̼� ���� X �ִϸ��̼��� �پ������ ����
        double decelerationEndTime = animinfo.AnimLength * animinfo.DecelerationRatio;

        //Ŭ���̾�Ʈ�� �i�ư����� �ִϸ��̼Ǳ���
        double remainingAnimTime = decelerationEndTime - serverPreTime;

        //Ŭ���̾�Ʈ�� �i�ư��� ���ؼ� ȣ��Ʈ���� ��ŭ �ִϸ��̼��� ����� �ϴ���
        double catchAnimSpeed = Math.Clamp(decelerationEndTime/ remainingAnimTime, _normalAnimSpeed, _maxAnimSpeed);

        if (indicatorCon != null)
        {
            indicatorCon.OnIndicatorDone += () => { _finishedIndicatorDuration = true; };
        }
        else
        {
            StartCoroutine(UpdateIndicatorDurationTime(animinfo.AddIndicatorDuration, animinfo.AnimLength, nowTime));
        }
        while (elaspedTime <=animinfo.AnimLength)
        {
            double currentNetTime = NetworkManager.Singleton.ServerTime.Time;
            double deltaTime = (currentNetTime - nowTime);
            nowTime = currentNetTime;

            //���⼭ ���� �ε������Ͱ� �� ���ƴ��� Ȯ���ؾ���
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


    IEnumerator UpdateIndicatorDurationTime(float indicatorAddduration, float animLength, double prevNetTime)
    {
        _finishedIndicatorDuration = false;
        float elapsedTime = 0f;
        while (elapsedTime <= indicatorAddduration + animLength)
        {
            double currentNetTime = NetworkManager.Singleton.ServerTime.Time;
            float deltaTime = (float)(currentNetTime - prevNetTime);
            elapsedTime += deltaTime;
            yield return null;
        }
        _finishedIndicatorDuration = true;
    }
}