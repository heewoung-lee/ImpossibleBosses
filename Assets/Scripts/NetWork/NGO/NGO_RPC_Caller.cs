using Unity.Netcode;
using UnityEngine;

public class NGO_RPC_Caller : NetworkBehaviour
{

    NetworkManager _networkManager;
    NetworkManager RelayNetworkManager
    {
        get
        {
            if (_networkManager == null)
            {
                _networkManager = Managers.RelayManager.NetWorkManager;
            }
            return _networkManager;
        }
    }


    [Rpc(SendTo.Server)]
    public void DeSpawn_NetWorkOBJServerRpc(ulong networkID, RpcParams rpcParams = default)
    {
        RelayNetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkID, out NetworkObject ngo);
        ngo.Despawn(true);
    }
    //[Rpc(SendTo.Server)]
    //public void Spawn_Loot_Item(IItem iteminfo, Func<IItem, GameObject> spawnEvent, bool destroyOption = false)
    //{
    //    GameObject obj = spawnEvent?.Invoke(iteminfo);
    //    RelayManager.SpawnNetworkOBJ(obj, Managers.LootItemManager.ItemRoot);
    //}

}
