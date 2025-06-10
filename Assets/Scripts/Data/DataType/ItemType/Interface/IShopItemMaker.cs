using UnityEngine;

namespace Data.DataType.ItemType.Interface
{
    public interface IShopItemMaker
    {
        public UI_ShopItemComponent MakeShopItemComponent(int itemPrice, Transform parent = null, int itemCount = 1,
            string name = null, string path = null);
    }
}