using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_LoadingScene_RPC_Caller : NetworkBehaviour
{
    NetworkLoadingScene _networkLoadingScene;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _networkLoadingScene = Managers.SceneManagerEx.GetCurrentScene as NetworkLoadingScene;
        StartCoroutine(_networkLoadingScene.LoadingSceneProcess(CalltoDoneLoadingRpc));
    }
    [Rpc(SendTo.Server)]
    public void CalltoDoneLoadingRpc()
    {
        ProgressUpdateToClientRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void ProgressUpdateToClientRpc()
    {
        _networkLoadingScene.DoneLoadScenePlayerCount++;
        Debug.Log($"���� �Ϸ�� �÷��̾� ī��Ʈ {_networkLoadingScene.DoneLoadScenePlayerCount}");
    }

}
