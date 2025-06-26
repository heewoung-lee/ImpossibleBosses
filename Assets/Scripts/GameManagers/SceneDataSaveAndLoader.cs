using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using GameManagers.Interface;
using GameManagers.Interface.ItemDataManager;
using GameManagers.Interface.UIManager;
using UI.SubItem;
using Util;
using Zenject;

namespace GameManagers
{
    public class SceneDataSaveAndLoader 
    {
        private Dictionary<EquipmentSlotType, IteminfoStruct> _equipmentSlotData = new Dictionary<EquipmentSlotType, IteminfoStruct>();
        private List<IteminfoStruct> _inventoryItemList = new List<IteminfoStruct>();
        [Inject] private IItemGetter _itemGetter;

        public void SaveInventoryItem(List<IteminfoStruct> saveItemlist)
        {
            _inventoryItemList.AddRange(saveItemlist);
        }

        public bool TryGetLoadInventoryItem(IUIPopupManager popupManager,out List<UIItemComponentInventory> loadInventory)
        {
            loadInventory = new List<UIItemComponentInventory>();
            if (_inventoryItemList == null || _inventoryItemList.Count <= 0)
                return false;

            foreach (IteminfoStruct iteminfo in _inventoryItemList)
            {
                IItem item = _itemGetter.GetItemByItemNumber(iteminfo.ItemNumber);
                UIItemComponentInventory inventoryitem = item.MakeInventoryItemComponent(popupManager);
                inventoryitem.SetINewteminfo(iteminfo);
                loadInventory.Add(inventoryitem);
            }
            _inventoryItemList.Clear();
            return true;
        }



        public void SaveEquipMentData(KeyValuePair<EquipmentSlotType, UIItemComponentInventory> equipValue)
        {

            IteminfoStruct iteminfo = new IteminfoStruct(equipValue.Value);
            _equipmentSlotData.Add(equipValue.Key,iteminfo);
            //여기에 그냥 값만 담아야 하고 나중에 열었을때 아이템으로 던저야 할것 같다
        }

        public bool TryGetLoadEquipMentData(EquipmentSlotType equipMentType,out IteminfoStruct equipItem)
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
}
