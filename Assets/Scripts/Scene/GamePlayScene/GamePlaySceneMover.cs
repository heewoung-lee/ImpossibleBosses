using System;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

public class GamePlaySceneMover : ISceneMover
{
    public void MoveScene()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;

        Managers.RelayManager.NGO_RPC_Caller.ResetManagersRpc();
        Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
        Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene, null, null);
    }
}
