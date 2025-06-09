using System.Collections.Generic;
using UnityEngine;

public class SceneDataSaveAndLoader 
{
    private Dictionary<Equipment_Slot_Type, IteminfoStruct> _equipmentSlotData = new Dictionary<Equipment_Slot_Type, IteminfoStruct>();
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
