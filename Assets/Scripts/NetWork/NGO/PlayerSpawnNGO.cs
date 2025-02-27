using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawnNGO : NetworkBehaviourBase
{
    private GameObject _player;

    private RelayManager _relayManager;
    protected override void AwakeInit()
    {
        _relayManager = Managers.RelayManager;
    }

    protected override void StartInit()
    {
        //Vector3 targetPosition = Managers.GameManagerEx.SpawnPoint.transform
        //    .GetChild(Random.Range(0, Managers.GameManagerEx.SpawnPoint.transform.childCount)).position;

        //Vector3 targetPosition = Vector3.zero;
        //_player.GetComponent<NavMeshAgent>().Warp(targetPosition);//낑김방지를 위한 함수실행
    }

    //protected override void OnNetworkPostSpawn()
    //{
    //    base.OnNetworkPostSpawn();
    //    if (IsOwner)
    //    {
    //        string choicePlayer = Enum.GetName(typeof(Define.PlayerClass), _relayManager.ChoicePlayerCharacter);
    //        _player = Managers.GameManagerEx.Spawn($"Prefab/Player/{choicePlayer}");
    //        Vector3 targetPosition = Vector3.zero;
    //        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);
    //        _relayManager.SpawnNetworkOBJ(OwnerClientId, _player, _relayManager.NGO_ROOT.transform);
    //        Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(_player);
    //    }
    //}
}