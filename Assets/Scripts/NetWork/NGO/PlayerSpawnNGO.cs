using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawnNGO : NetworkBehaviourBase
{
    private RelayManager _relayManager;
    GameObject _player;
    protected override void AwakeInit()
    {
        _relayManager = Managers.RelayManager;
    }

    protected override void StartInit()
    {
       RequestSpawnPlayerServerRpc(_relayManager.NetWorkManager.LocalClientId);
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong requestingClientId)
    {
        string choicePlayer = Enum.GetName(typeof(Define.PlayerClass), _relayManager.ChoicePlayerCharacter);
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}Base");
        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        _player = _relayManager.SpawnNetworkOBJ(requestingClientId, _player, parent: _relayManager.NGO_ROOT.transform);
    }

}