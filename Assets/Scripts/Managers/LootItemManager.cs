using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LootItemManager
{
    private GameObject _itemRoot;

    public Transform ItemRoot
    {
        get
        {
            if(_itemRoot == null)
            {
                _itemRoot = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/ItemRootNetwork");
            }
            return _itemRoot.transform;
        }
    }


    private GameObject _temporaryInventory;

    public Transform TemporaryInventory
    {
        get
        {
            if(_temporaryInventory == null)
            {
                _temporaryInventory = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/LootItemRootNetwork");
            }
            return _temporaryInventory.transform;  
        }
    }


    public Transform GetItemComponentPosition(UI_Player_Inventory playerInventory)
    {
        if (playerInventory.gameObject.activeSelf)
        {
            return playerInventory.ItemInventoryTr;
        }
        return TemporaryInventory;
    }



    public void LoadItemsFromLootStorage(Transform itemInventoryTr)
    {
        if (TemporaryInventory.childCount <= 0)
            return;

        Transform[] lootItemTransform = new Transform[TemporaryInventory.childCount];

        for(int tempindex = 0; tempindex<lootItemTransform.Length; tempindex++)
        {
            lootItemTransform[tempindex] = TemporaryInventory.GetChild(tempindex).transform;
        }

        for (int index = 0; index< lootItemTransform.Length; index++)
        {
            Transform child = lootItemTransform[index];
            UI_ItemComponent_Inventory lootItem = child.GetComponent<UI_ItemComponent_Inventory>();
            if (lootItem != null)
            {
                lootItem.transform.SetParent(itemInventoryTr);

                if (lootItem is UI_ItemComponent_Consumable)
                {
                    (lootItem as UI_ItemComponent_Consumable).CombineConsumableItems();
                }
            }
        }
    }
}