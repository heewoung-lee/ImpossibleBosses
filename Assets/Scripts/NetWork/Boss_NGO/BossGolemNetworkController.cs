using BehaviorDesigner.Runtime;
using JetBrains.Annotations;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BossGolemNetworkController : NetworkBehaviourBase
{
    private BehaviorTree _bossBehaviourTree;
    private BossAttack _bossAttackNode;
    private BossGolemController _bossController;

    private NetworkVariable<float> _animSpeed = new NetworkVariable<float>
    (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    protected override void AwakeInit()
    {
        _bossController= GetComponent<BossGolemController>();
        _bossBehaviourTree = GetComponent<BehaviorTree>();
        _bossAttackNode = _bossBehaviourTree.FindTask<BossAttack>();
    }
    public float AnimSpeed
    {
        get => _animSpeed.Value;

        set
        {
            if (IsHost == false) return;
            _animSpeed.Value = value;
        }
    }
    protected override void StartInit()
    {
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.GameManagerEx.SetBossMonster(gameObject);
        _animSpeed.OnValueChanged += OnAnimSpeedValueChanged;

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

    private void OnAnimSpeedValueChanged(float previousValue, float newValue)
    {
        _bossController.HostAnimSpeedChange(newValue);
    }

}
