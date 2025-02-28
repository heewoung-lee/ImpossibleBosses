using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene
{
    private GameObject _player;
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
        _player = Managers.GameManagerEx.Spawn($"Prefabs/Player/{choicePlayer}");
        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);//³©±è¹æÁö
        _relayManager.SpawnNetworkOBJ(_relayManager.NetWorkManager.LocalClientId, _player, _relayManager.NGO_ROOT.transform);
        Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(_player);
    }

    public override void Clear()
    {
        
    }

}