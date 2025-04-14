using Unity.Netcode;
using UnityEngine;

public class NGO_LoadingScene_RPC_Caller : NetworkBehaviour
{

    NetworkVariable<int> doneClientLoadingSceneCount = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    NetworkLoadingScene _networkLoadingScene;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _networkLoadingScene = Managers.SceneManagerEx.GetCurrentScene as NetworkLoadingScene;
    }

    [Rpc(SendTo.Server)]
    public void CalltoDoneLoadingRpc()
    {
        doneClientLoadingSceneCount.Value++;
        ProgressUpdateToClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ProgressUpdateToClientRpc()
    {
        //TODO:클라이언트의 프로그레스 바 업데이트
    }

}
