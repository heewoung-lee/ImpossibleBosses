using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GamePlaySceneMover : ISceneMover
{
    public void MoveScene()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;

        Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
        Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene, null, SetPlayerPostion);
        Managers.RelayManager.NGO_RPC_Caller.ResetManagersRpc();

        void SetPlayerPostion()
        {
            foreach (NetworkObject player in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
            {
                Vector3 pos = new Vector3(player.OwnerClientId, 0, 0);

                if (player.TryGetComponent(out NavMeshAgent agent))
                {
                    agent.Warp(pos);
                    player.GetComponent<PlayerInitalizeNGO>().SetForcePositionFromNetworkRpc(pos);
                }

            }
        }
    }
}
