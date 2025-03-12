using Unity.Netcode;
using UnityEngine;

public class NGO_RPC_Caller : NetworkBehaviour
{
    [Rpc(SendTo.Server)]
    public void DeSpawn_NetWorkOBJServerRpc(GameObject go, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log("이 RPC를 호출한 클라이언트 ID: " + clientId);
        if (go.TryGetComponent(out NetworkObject ngo))
        {
            ngo.Despawn(true);
        }
    }
    [Rpc(SendTo.Server)]
    public void Spawn_Object_ServerRpc(ulong clientId, GameObject obj, Transform parent = null, bool destroyOption = false)
    {
        Managers.RelayManager.SpawnNetworkOBJ(clientId, obj, parent, destroyOption);
    }

}
