using System.Collections.Generic;
using UnityEngine;

public class SceneDataSaveAndLoader 
{
    private Dictionary<Equipment_Slot_Type, IteminfoStruct> _equipmentSlotData = new Dictionary<Equipment_Slot_Type, IteminfoStruct>();
    private List<IteminfoStruct> _inventoryItemList = new List<IteminfoStruct>();


    public void SaveInventoryItem(List<IteminfoStruct> saveItemlist)
    {
        _inventoryItemList.AddRange(saveItemlist);
    }

    public bool TryGetLoadInventoryItem(out List<UI_ItemComponent_Inventory> loadInventory)
    {
        loadInventory = new List<UI_ItemComponent_Inventory>();
        if (_inventoryItemList == null || _inventoryItemList.Count <= 0)
            return false;

        foreach (IteminfoStruct iteminfo in _inventoryItemList)
        {
            IItem item = Managers.ItemDataManager.GetItem(iteminfo.ItemNumber);
            UI_ItemComponent_Inventory inventoryitem = item.MakeInventoryItemComponent();
            inventoryitem.SetINewteminfo(iteminfo);
            loadInventory.Add(inventoryitem);
        }
        _inventoryItemList.Clear();
        return true;
    }



    public void SaveEquipMentData(KeyValuePair<Equipment_Slot_Type, UI_ItemComponent_Inventory> equipValue)
    {

        IteminfoStruct iteminfo = new IteminfoStruct(equipValue.Value);
        _equipmentSlotData.Add(equipValue.Key,iteminfo);
        //여기에 그냥 값만 담아야 하고 나중에 열었을때 아이템으로 던저야 할것 같다
    }

    public bool TryGetLoadEquipMentData(Equipment_Slot_Type equipMentType,out IteminfoStruct equipItem)
    {
        equipItem = default;
        if (_equipmentSlotData.TryGetValue(equipMentType,out IteminfoStruct iteminfo))
        {
            equipItem = iteminfo;
            _equipmentSlotData.Remove(equipMentType);
            return true;
        }
        return false;
    }


}
