using GameManagers;
using UI.SubItem;
using UnityEngine;

namespace Data.DataType.ItemType.Interface
{
    public interface IShopItemMaker
    {
        public UIShopItemComponent MakeShopItemComponent(UIManager uiManager,int itemPrice, Transform parent = null, int itemCount = 1,
            string name = null, string path = null);
    }
}