using System;
using System.Collections.Generic;
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
        Managers.NGO_PoolManager.Create_NGO_Pooling_Object();//��Ʈ��ũ ������Ʈ Ǯ�� ����
    }
    private void HostSpawnObject()
    {
        if (IsHost == false)
            return;
        Managers.RelayManager.SpawnToRPC_Caller();
        Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/VFX_Root_NGO");
        RequestSpawnToNPC(new List<(string, Vector3)>() //������ �׽�Ʈ�� ���� ť��
        {
           {("Prefabs/NPC/Damage_Test_Dummy",new Vector3(10f,0,-2.5f))}
        });
        Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/Scene_NGO/NGO_BossRoomEntrance",Managers.RelayManager.NGO_ROOT.transform);
        Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller", Managers.RelayManager.NGO_ROOT.transform);
    }

    private void RequestSpawnToNPC(List<(string, Vector3)> npcPathAndTr)
    {
        foreach ((string, Vector3) npcdata in npcPathAndTr)
        {
            Managers.RelayManager.SpawnNetworkOBJ($"{npcdata.Item1}", Managers.RelayManager.NGO_ROOT.transform, position: npcdata.Item2);
        }
    }
    protected override void StartInit()
    {
    }
}