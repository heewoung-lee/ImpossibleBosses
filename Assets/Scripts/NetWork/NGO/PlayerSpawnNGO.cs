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
        if (IsAvailableMockUnitTest())
        {
            PlayerSpawn();
            return;
        }

        Managers.RelayManager.NetWorkManager.SceneManager.OnLoadComplete += SpawnPlayer_OnLoadComplete;
        if (IsHost)
            PlayerSpawn();
    }

    private void SpawnPlayer_OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if(clientId == _relayManager.NetWorkManager.LocalClientId)
            PlayerSpawn();
    }

    private void PlayerSpawn()
    {
        string choicePlayer = Managers.RelayManager.ChoicePlayerCharacter.ToString();
        RequestSpawnPlayerServerRpc(_relayManager.NetWorkManager.LocalClientId, choicePlayer);
    }
    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong requestingClientId,string choicePlayer)
    {
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}base");
        Vector3 targetPosition = new Vector3(1*requestingClientId, 0, 1);
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        _relayManager.SpawnNetworkOBJ(requestingClientId, _player, parent: _relayManager.NGO_ROOT.transform);
    }



    private bool IsAvailableMockUnitTest()
    {
        PlaySceneMockUnitTest mockUnitTest = FindAnyObjectByType<PlaySceneMockUnitTest>();
        if (mockUnitTest == null || mockUnitTest.enabled)
        {
            return true;
        }
        return false; 
    }
}