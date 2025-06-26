using System;
using Data.DataType.ItemType.Interface;

namespace GameManagers.Interface.ItemDataManager
{
    public interface IItemGetter
    {
        public IItem GetItemByItemNumber(int itemNumber);
        public IItem GetRandomItem(Type itemtype);
        public IItem GetRandomItemFromAll();
    }
}
