using System;
using System.Collections.Generic;
using GameManagers;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NGO_GamePlaySceneSpawn : NetworkBehaviourBase
{
    private RelayManager _relayManager;
    GameObject _player;
    protected override void AwakeInit()
    {
        _relayManager = Managers.RelayManager;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        HostSpawnObject();
        Managers.NgoPoolManager.Create_NGO_Pooling_Object();//네트워크 오브젝트 풀링 생성
    }
    private void HostSpawnObject()
    {
        if (IsHost == false)
            return;
        Managers.RelayManager.SpawnToRPC_Caller();
        Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/VFX_Root_NGO");
        RequestSpawnToNPC(new List<(string, Vector3)>() //데미지 테스트용 더미 큐브
        {
           {("Prefabs/NPC/Damage_Test_Dummy",new Vector3(10f,0,-2.5f))}
        });
        Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_BossRoomEntrance",Managers.RelayManager.NgoRoot.transform);
        Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller", Managers.RelayManager.NgoRoot.transform);
    }

    private void RequestSpawnToNPC(List<(string, Vector3)> npcPathAndTr)
    {
        foreach ((string, Vector3) npcdata in npcPathAndTr)
        {
            Managers.RelayManager.SpawnNetworkObj($"{npcdata.Item1}", Managers.RelayManager.NgoRoot.transform, position: npcdata.Item2);
        }
    }
    protected override void StartInit()
    {
    }
}