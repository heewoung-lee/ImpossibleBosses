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
    //public void Spawn_Loot_ItemRpc(IItemStruct itemStruct, bool destroyOption = true)
    //{

    //    //여기에서 itemStruct를 IItem으로 변환
    //    GameObject lootItem;

    //    switch (itemStruct.Item_Type)
    //    {
    //        case ItemType.Equipment:

    //            itemStruct.Ty
    //            lootItem = Managers.ResourceManager.InstantiatePrefab();
    //            lootItem = GetLootingItemObejct();
    //            break;
    //        case ItemType.Consumable:

    //            break;
    //        case ItemType.ETC:
    //            break;
    //    }


    //    lootItem.GetComponent<UI_ItemComponent_Inventory>().GetLootingItemObejct(iteminfo);
    //     = GetLootingItemObejct(iteminfo);
    //    RelayManager.SpawnNetworkOBJ(obj, Managers.LootItemManager.ItemRoot, destroyOption);
    //}

}
