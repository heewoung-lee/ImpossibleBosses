using Data.DataType.ItemType.Interface;
using UnityEngine;

namespace GameManagers.Interface.ItemDataManager
{
    public interface ILootItemGetter
    {
        public GameObject GetEquipLootItem(IItem iteminfo);
        public GameObject GetConsumableLootItem(IItem iteminfo);
    }
}
