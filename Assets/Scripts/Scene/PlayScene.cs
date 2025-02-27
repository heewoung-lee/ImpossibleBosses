using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene
{
    private GameObject _player;

    private RelayManager _relayManager;

    public Define.PlayerClass SpawnPlayerClass;

    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    protected override void StartInit()
    {
        base.StartInit();
        //Vector3 targetPosition = Managers.GameManagerEx.SpawnPoint.transform
        //    .GetChild(Random.Range(0, Managers.GameManagerEx.SpawnPoint.transform.childCount)).position;

        //Vector3 targetPosition = Vector3.zero;
        //_player.GetComponent<NavMeshAgent>().Warp(targetPosition);//낑김방지를 위한 함수실행
        string choicePlayer = Enum.GetName(typeof(Define.PlayerClass), _relayManager.ChoicePlayerCharacter);
        _player = Managers.ResourceManager.InstantiatePrefab($"Player/{choicePlayer}");
        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
        _relayManager.SpawnNetworkOBJ(_relayManager.NetWorkManager.LocalClientId, _player, _relayManager.NGO_ROOT.transform);
        Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(_player);
    }

    public override void Clear()
    {
        
    }

    protected override void AwakeInit()
    {
        // _player = Managers.GameManagerEx.Spawn($"Prefabs/Player/{SpawnPlayerClass}"); PlayerSpawnNGO 스크립트에서 플레이어 스폰
        //gameObject.GetOrAddComponent<PlayerSpawnNGO>();

        _relayManager = Managers.RelayManager;
       
    }

}