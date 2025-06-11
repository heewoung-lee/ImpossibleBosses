using System;
using System.Collections.Generic;
using GameManagers;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NGO_BattleSceneSpawn : NetworkBehaviourBase
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
    }
    private void HostSpawnObject()
    {
        if (IsHost == false)
            return;
        Managers.RelayManager.SpawnToRPC_Caller();
        Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/VFX_Root_NGO");
        RequestSpawnToNPC(new List<(string, Vector3)>
        {
           ("Prefabs/Enemy/Boss/Character/StoneGolem",new Vector3(10f,0f,10f))
        });
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