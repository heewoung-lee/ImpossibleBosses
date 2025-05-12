using BehaviorDesigner.Runtime;
using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BossGolemNetworkController : NetworkBehaviourBase
{
    private BehaviorTree _bossBehaviourTree;
    private BossGolemController _bossController;
    private bool _finishedAttack = false;
    private Coroutine _animationCoroutine;
    public bool FinishAttack
    {
        get => _finishedAttack;
        private set => _finishedAttack = value;
    }
    protected override void AwakeInit()
    {
        _bossController= GetComponent<BossGolemController>();
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
    public void StartAnimChagnedRpc(float animLength, float preTime, float animStopThreshold)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(UpdateAnimCorutine(animLength,preTime, animStopThreshold));
    }
    IEnumerator UpdateAnimCorutine(float animLength, float preTime, float animStopThreshold,float startAnimSpeed = 1f)
    {
        float elaspedTime = 0f;
        FinishAttack = false;
        while (elaspedTime <= animLength)
        {
            elaspedTime += Time.unscaledDeltaTime * _bossController.Anim.speed;
            if (_bossController.TryGetAnimationSpeed(elaspedTime, animLength, preTime, out float animspeed, animStopThreshold, startAnimSpeed))
            {
                _bossController.Anim.speed = 1;
            }
            yield return null;
        }
        FinishAttack = true;
    }
}
