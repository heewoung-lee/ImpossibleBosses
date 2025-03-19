using System;
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
                _networkManager = Managers.RelayManager.NetworkManagerEx;
            }
            return _networkManager;
        }
    }


    [Rpc(SendTo.Server)]
    public void DeSpawnByIDServerRpc(ulong networkID, RpcParams rpcParams = default)
    {
        RelayNetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkID, out NetworkObject ngo);
        ngo.Despawn(true);
    }

    [Rpc(SendTo.Server,RequireOwnership =false)]
    public void DeSpawnByReferenceServerRpc(NetworkObjectReference ngoRef, RpcParams rpcParams = default)
    {
        if (ngoRef.TryGet(out NetworkObject ngo))
        {
            ngo.Despawn(true);
        }
    }


    [Rpc(SendTo.Server)]
    public void Spawn_Loot_ItemRpc(IteminfoStruct itemStruct, Vector3 dropPosition, bool destroyOption = true)
    {
        //여기에서 itemStruct를 IItem으로 변환
        GameObject networkLootItem = null;
        IItem iteminfo = Managers.ItemDataManager.GetItem(itemStruct.ItemNumber);
        switch (itemStruct.Item_Type)
        {
            case ItemType.Equipment:
                networkLootItem = GetEquipLootItem(iteminfo);
                break;
            case ItemType.Consumable:
                networkLootItem = GetConsumableLootItem(iteminfo);
                break;
            case ItemType.ETC:
                break;
        }
        networkLootItem.GetComponent<LootItem>().SetPosition(dropPosition);
        GameObject rootItem = Managers.RelayManager.SpawnNetworkOBJ(networkLootItem, Managers.LootItemManager.ItemRoot);
        NetworkObjectReference rootItemRef = Managers.RelayManager.GetNetworkObject(rootItem);
        SetDropItemInfoRpc(itemStruct, rootItemRef);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetDropItemInfoRpc(IteminfoStruct itemStruct, NetworkObjectReference rootitemRef)
    {
        if (rootitemRef.TryGet(out NetworkObject ngo))
        {
            IItem iteminfo = Managers.ItemDataManager.GetItem(itemStruct.ItemNumber);
            ngo.GetComponent<LootItem>().SetIteminfo(iteminfo);
        }
    }

    private GameObject GetEquipLootItem(IItem iteminfo)
    {
        GameObject lootItem;
        switch ((iteminfo as ItemEquipment).Equipment_Slot)
        {
            case Equipment_Slot_Type.Helmet:
            case Equipment_Slot_Type.Armor:
                lootItem = Managers.ResourceManager.InstantiatePrefab("NGO/LootingItem/Shield");
                break;
            case Equipment_Slot_Type.Weapon:
                lootItem = Managers.ResourceManager.InstantiatePrefab("NGO/LootingItem/Sword");
                break;
            default:
                lootItem = Managers.ResourceManager.InstantiatePrefab("NGO/LootingItem/Bag");
                break;
        }
        lootItem.GetComponent<LootItem>().SetIteminfo(iteminfo);
        return lootItem;
    }

    private GameObject GetConsumableLootItem(IItem iteminfo)
    {
        GameObject lootitem = Managers.ResourceManager.InstantiatePrefab("NGO/LootingItem/Potion");
        lootitem.GetComponent<LootItem>().SetIteminfo(iteminfo);
        return lootitem;
    }
}
