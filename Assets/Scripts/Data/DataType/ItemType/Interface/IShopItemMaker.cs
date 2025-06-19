using GameManagers;
using GameManagers.Interface;
using UI.SubItem;
using UnityEngine;

namespace Data.DataType.ItemType.Interface
{
    public interface IShopItemMaker
    {
        public UIShopItemComponent MakeShopItemComponent(IUISubItem subItemManager,int itemPrice, Transform parent = null, int itemCount = 1,
            string name = null, string path = null);
    }
}