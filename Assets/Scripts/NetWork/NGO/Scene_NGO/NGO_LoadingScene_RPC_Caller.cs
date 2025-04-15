using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_LoadingScene_RPC_Caller : NetworkBehaviour
{
    NetworkLoadingScene _networkLoadingScene;
    //��� ���α׷��� ������ �뵵�� ���
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
