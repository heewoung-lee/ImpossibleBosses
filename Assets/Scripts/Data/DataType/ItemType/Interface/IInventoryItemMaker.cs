using GameManagers;
using GameManagers.Interface;
using UI.SubItem;
using UnityEngine;

namespace Data.DataType.ItemType.Interface
{
    public interface IInventoryItemMaker
    {
        public UIItemComponentInventory MakeItemComponentInventory(IUISubItem subItemManager,Transform parent = null, int itemCount = 1, string name = null, string path = null);
    }
}
