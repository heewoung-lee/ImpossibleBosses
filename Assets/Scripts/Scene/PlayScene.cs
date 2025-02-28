using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene
{
    private RelayManager _relayManager;
    public Define.PlayerClass SpawnPlayerClass;
    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    protected override void AwakeInit()
    {
        _relayManager = Managers.RelayManager;
    }
    protected override void StartInit()
    {
        base.StartInit();
        string choicePlayer = Enum.GetName(typeof(Define.PlayerClass), _relayManager.ChoicePlayerCharacter);
        GameObject player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}");
        Managers.GameManagerEx.SetPlayer(player);
        Vector3 targetPosition = Vector3.zero;
        player.GetComponent<NavMeshAgent>().Warp(targetPosition);//³©±è¹æÁö
        _relayManager.SpawnNetworkOBJ(_relayManager.NetWorkManager.LocalClientId, player);
        Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(player);
    }

    public override void Clear()
    {

    }

}