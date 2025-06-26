using Data.DataType.ItemType.Interface;
using UnityEngine;

namespace GameManagers.Interface.ItemDataManager
{
    public interface IItemGradeBorder
    {
        public Sprite GetGradeBorder(ItemGradeType gradeType);
    }
}
