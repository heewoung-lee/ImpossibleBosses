using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene
{
    private GameObject _player;
    protected override void StartInit()
    {
        base.StartInit();

        currentScene = Define.Scene.Game;
        //Vector3 targetPosition = Managers.GameManagerEx.SpawnPoint.transform
        //    .GetChild(Random.Range(0, Managers.GameManagerEx.SpawnPoint.transform.childCount)).position;

        Vector3 targetPosition = Vector3.zero;
        _player.GetComponent<NavMeshAgent>().Warp(targetPosition);//��������� ���� �Լ�����



    }

    public override void Clear()
    {
        
    }

    protected override void AwakeInit()
    {
        _player = Managers.GameManagerEx.Spawn("Prefabs/Player/Player");
    }
}