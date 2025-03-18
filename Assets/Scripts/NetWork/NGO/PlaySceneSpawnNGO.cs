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

        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadComplete += SpawnPlayer_OnLoadComplete;
        if (IsHost)
            SpawnObject();
    }

    private void SpawnPlayer_OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if (clientId == _relayManager.NetworkManagerEx.LocalClientId)
            SpawnObject();
    }

    private void SpawnObject()
    {
        string choicePlayer = Managers.RelayManager.ChoicePlayerCharacter.ToString();
        RequestSpawnPlayerServerRpc(_relayManager.NetworkManagerEx.LocalClientId, choicePlayer);
        RequestSpawnToNPC(new List<(string, Vector3)>()
        {
           {("Dummy_Test_Cube",new Vector3(10f,0.72f,-2.5f))}
        });
    }
    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
    }

    private void RequestSpawnToNPC(List<(string, Vector3)> npcPathAndTr)
    {
        if (IsHost == false)
            return;

        foreach ((string, Vector3) npcdata in npcPathAndTr)
        {
            GameObject dummy_cube = Managers.ResourceManager.InstantiatePrefab($"{npcdata.Item1}");
            dummy_cube.transform.position = npcdata.Item2;
            Managers.RelayManager.SpawnNetworkOBJ(dummy_cube,Managers.RelayManager.NGO_ROOT.transform);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong requestingClientId, string choicePlayer)
    {
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}Base");
        Vector3 targetPosition = new Vector3(1 * requestingClientId, 0, 1);
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        _relayManager.SpawnNetworkOBJInjectionOnwer(requestingClientId, _player,_relayManager.NGO_ROOT.transform);
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