using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
        if (IsOwner == false)
            return;

        string choicePlayer = Enum.GetName(typeof(Define.PlayerClass), _relayManager.ChoicePlayerCharacter);
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}");
        Managers.GameManagerEx.SetPlayer(_player);
        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(_player);
        SpawntoPlayerClientRpc(requestingClientId,$"Player/{choicePlayer}Base");
    }


    [ClientRpc]
    private void SpawntoPlayerClientRpc(ulong requestingClientId,string notOnwerPath)
    {

        if (IsOwner)
        {
            _player.GetComponent<NetworkObject>().SpawnAsPlayerObject(requestingClientId);
        }
        else
        {
            _player = Managers.ResourceManager.InstantiatePrefab($"{notOnwerPath}");
            Managers.GameManagerEx.SetPlayer(_player);
            Vector3 targetPosition = Vector3.zero;
            _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
            _player.GetComponent<NetworkObject>().SpawnAsPlayerObject(requestingClientId);
        }

        _player = _relayManager.SpawnNetworkOBJ(OwnerClientId, _player,parent: _relayManager.NGO_ROOT.transform);
    }


}