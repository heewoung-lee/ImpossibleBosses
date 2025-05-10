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
    public bool FinishAttack = false;

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
    public void StartAnimChagnedRpc(float animLength, float preTime)
    {
        StartCoroutine(UpdateAnimCorutine(animLength,preTime));
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartAnimChagnedRpc(float animLength, float preTime, float startAnimSpeed)
    {
        StartCoroutine(UpdateAnimCorutine(animLength, preTime, startAnimSpeed));
    }

    IEnumerator UpdateAnimCorutine(float animLength, float preTime, float startAnimSpeed = 1f)
    {
        float elaspedTime = 0f;
        FinishAttack = false;
        while (elaspedTime <= animLength)
        {
            elaspedTime += Time.deltaTime * _bossController.Anim.speed;
            if (_bossController.TryGetAnimationSpeed(elaspedTime, animLength, preTime, out float animspeed, startAnimSpeed))
            {
                _bossController.Anim.speed = 1;
            }
            yield return null;
        }
        FinishAttack = true;
    }
}
