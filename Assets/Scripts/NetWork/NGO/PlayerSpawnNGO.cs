using System;
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
        if (IsOwner)
        {
            SpawntoPlayerServerRpc();
        }
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
    } 

    [ServerRpc]
    private void SpawntoPlayerServerRpc()
    {
        string choicePlayer = Enum.GetName(typeof(Define.PlayerClass), _relayManager.ChoicePlayerCharacter);
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}");
        Managers.GameManagerEx.SetPlayer(_player);
        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(_player);
        SpawntoPlayerClientRpc();
    }


    [ClientRpc]
    private void SpawntoPlayerClientRpc()
    {
        _player = _relayManager.SpawnNetworkOBJ(OwnerClientId, _player, _relayManager.NGO_ROOT.transform);
    }

}