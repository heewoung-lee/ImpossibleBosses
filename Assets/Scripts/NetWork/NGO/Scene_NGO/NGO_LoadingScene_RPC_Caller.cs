using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_LoadingScene_RPC_Caller : NetworkBehaviour
{
    NetworkLoadingScene _networkLoadingScene;
    //얘는 프로그레스 돌리는 용도로 사용
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _networkLoadingScene = Managers.SceneManagerEx.GetCurrentScene as NetworkLoadingScene;
    }
    [Rpc(SendTo.Server)]
    public void CalltoDoneLoadingRpc()
    {
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void ProgressUpdateToClientRpc()
    {
    }
}
