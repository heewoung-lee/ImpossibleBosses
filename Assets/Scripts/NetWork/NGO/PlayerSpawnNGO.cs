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
        Managers.RelayManager.NetWorkManager.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;


    }

    private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        string choicePlayer = Managers.RelayManager.ChoicePlayerCharacter.ToString();
        RequestSpawnPlayerServerRpc(_relayManager.NetWorkManager.LocalClientId, choicePlayer);
    }

    //public void SpawnPlayer()
    //{
    //    string choicePlayer = Managers.RelayManager.ChoicePlayerCharacter.ToString();
    //    RequestSpawnPlayerServerRpc(_relayManager.NetWorkManager.LocalClientId, choicePlayer);
    //}

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong requestingClientId,string choicePlayer)
    {
        //string choicePlayer = Define.PlayerClass.Fighter.ToString();
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}base");
        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        _relayManager.SpawnNetworkOBJ(requestingClientId, _player, parent: _relayManager.NGO_ROOT.transform);
    }

}