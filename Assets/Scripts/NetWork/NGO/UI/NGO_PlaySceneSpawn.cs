using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NGO_PlaySceneSpawn : NetworkBehaviourBase
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
        if (IsAvailableMockUnitTest(out PlaySceneMockUnitTest mockUnitTest))
        {
            Debug.Log($"{mockUnitTest.PlayerClass}캐릭터이름");
            MockUnitSpawnPlayerCharacter(mockUnitTest.PlayerClass);
            HostSpawnObject();
            return;

            void MockUnitSpawnPlayerCharacter(Define.PlayerClass playerclass)
            {
                string choicePlayer = playerclass.ToString();
                Managers.RelayManager.SetPlayerClassforMockUnitTest(playerclass);
                RequestSpawnPlayerServerRpc(_relayManager.NetworkManagerEx.LocalClientId, choicePlayer);
            }
        }
        HostSpawnObject();
    }
    private void HostSpawnObject()
    {
        if (IsHost == false)
            return;
        Managers.RelayManager.SpawnToRPC_Caller();
        Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/VFX_Root_NGO");
        RequestSpawnToNPC(new List<(string, Vector3)>() //데미지 테스트용 더미 큐브
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

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong requestingClientId, string choicePlayer)
    {
        Vector3 targetPosition = new Vector3(1 * requestingClientId, 0, 1);
        _relayManager.SpawnNetworkOBJInjectionOnwer(requestingClientId, $"Prefabs/Player/{choicePlayer}Base", targetPosition, _relayManager.NGO_ROOT.transform);
    }

    private bool IsAvailableMockUnitTest(out PlaySceneMockUnitTest mockUnitTest)
    {
        mockUnitTest = FindAnyObjectByType<PlaySceneMockUnitTest>();
        if (mockUnitTest != null && mockUnitTest.enabled == true)
            return true;

        return false;
    }

    protected override void StartInit()
    {
    }
}