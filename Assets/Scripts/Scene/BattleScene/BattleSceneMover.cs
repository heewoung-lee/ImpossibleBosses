using System;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

public class BattleSceneMover : ISceneMover
{
    public void MoveScene()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;


        Debug.Log("무브 씬 호출");

        Managers.RelayManager.NGO_RPC_Caller.ResetManagersRpc();
        Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
        Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.BattleScene, null, null);

        //TODO: 여기에 씬이동이 완료되면 좌표로직 적기
    }
}
