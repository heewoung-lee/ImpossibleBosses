using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

public class PlaySceneSpawnNGO : NetworkBehaviourBase
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
            SpawnObject();
            return;
        }

        Managers.RelayManager.NetWorkManager.SceneManager.OnLoadComplete += SpawnPlayer_OnLoadComplete;
        if (IsHost)
            SpawnObject();
    }

    private void SpawnPlayer_OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if(clientId == _relayManager.NetWorkManager.LocalClientId)
            SpawnObject();
    }

    private void SpawnObject()
    {
        string choicePlayer = Managers.RelayManager.ChoicePlayerCharacter.ToString();
        RequestSpawnPlayerServerRpc(_relayManager.NetWorkManager.LocalClientId, choicePlayer);
        RequestSpawnDummyCube();
    }
    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
    }

    public void RequestSpawnDummyCube()
    {
        if (IsHost)
        {
            GameObject dummyCube = Managers.ResourceManager.InstantiatePrefab("Dummy_Test_Cube");
            Managers.RelayManager.SpawnNetworkOBJ(Managers.RelayManager.NetWorkManager.LocalClientId, dummyCube,Managers.RelayManager.NGO_ROOT.transform);
        }
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong requestingClientId,string choicePlayer)
    {
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}Base");
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