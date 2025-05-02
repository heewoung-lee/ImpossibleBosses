using BehaviorDesigner.Runtime;
using JetBrains.Annotations;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BossGolemNetworkController : NetworkBehaviourBase
{
    private BossGolemController _controller;

    private NetworkVariable<float> _animSpeed = new NetworkVariable<float>
    (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected override void AwakeInit()
    {
        _controller = GetComponent<BossGolemController>();
        _controller.AnimationSpeedChanged += AnimationSpeedChanged;
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
    private void AnimationSpeedChanged(float animSpeed)
    {
        AnimSpeed = animSpeed;
    }


    protected override void StartInit()
    {
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.GameManagerEx.SetBossMonster(gameObject);
        //_animSpeed.OnValueChanged += OnAnimSpeedValueChanged;


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
        _controller.HostAnimChange(newValue);
    }

}
