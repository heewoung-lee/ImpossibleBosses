using System.Collections.Generic;
using UnityEngine;

namespace Data.DataType.ItemType.Interface
{
    public enum ItemGradeType
    {
        Normal,
        Magic,
        Rare,
        Unique,
        Epic
    }
    public enum ItemType
    {
        Equipment,
        Consumable,
        ETC
    }
    public enum StatType
    {
        MaxHP,
        CurrentHp,
        Attack,
        Defence,
        MoveSpeed,
    }

    public interface IItem
    {
        public int ItemNumber { get; }
        public ItemType ItemType { get; }
        public ItemGradeType ItemGradeType { get; }
        public List<StatEffect> ItemEffects { get; }
        public string ItemName { get; }
        public string DescriptionText { get; }
        public string ItemIconSourceText { get; }
        public Dictionary<string,Sprite> ImageSource { get; }
    }
}