using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Item_Grade_Type
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
    public ItemType Item_Type { get; }
    public Item_Grade_Type Item_Grade { get; }
    public List<ItemEffect> ItemEffects { get; }
    public string ItemName { get; }
    public string DescriptionText { get; }
    public string ItemIconSourceText { get; }
    public Dictionary<string,Sprite> ImageSource { get; }

}