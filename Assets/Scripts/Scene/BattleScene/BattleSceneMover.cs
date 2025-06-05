using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BattleSceneMover : ISceneMover
{
    public void MoveScene()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;


        Managers.RelayManager.NGO_RPC_Caller.ResetManagersRpc();
        Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
        Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.BattleScene, null, SetPostion);


        void SetPostion()
        {
            foreach (NetworkObject player in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
            {
                if (player.TryGetComponent(out NavMeshAgent agent))
                {
                    agent.ResetPath();
                    agent.Warp(new Vector3(player.OwnerClientId, 0, 0));
                }
            }
        }
    }
}
