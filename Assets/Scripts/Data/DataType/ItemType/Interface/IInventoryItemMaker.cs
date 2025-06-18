using GameManagers;
using UI.SubItem;
using UnityEngine;

namespace Data.DataType.ItemType.Interface
{
    public interface IInventoryItemMaker
    {
        public UIItemComponentInventory MakeItemComponentInventory(UIManager uiManager,Transform parent = null, int itemCount = 1, string name = null, string path = null);
    }
}
