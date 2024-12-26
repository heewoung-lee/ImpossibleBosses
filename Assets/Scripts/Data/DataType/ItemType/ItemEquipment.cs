using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public enum Equipment_Slot_Type
{
    Helmet,
    Gauntlet,
    Shoes,
    Weapon,
    Ring,
    Armor
}
[Serializable]
public class ItemEquipment : Ikey<int>,IItem,IInventoryItemMaker,IItemDescriptionForm, IShopItemMaker
{
    public int itemNumber;
    public ItemType itemType = ItemType.Equipment;
    public string itemGradeText;
    public string equipment_SlotText;
    public List<ItemEffect> itemEffects = new List<ItemEffect>();
    public string itemName;
    public string descriptionText;
    public string itemIconSourceText;

    private Dictionary<string, Sprite> _imageSource = new Dictionary<string, Sprite>();

    public int ItemNumber => itemNumber;
    public ItemType Item_Type => itemType;
    public Item_Grade_Type Item_Grade => (Item_Grade_Type)System.Enum.Parse(typeof(Item_Grade_Type), itemGradeText);
    public Equipment_Slot_Type Equipment_Slot => (Equipment_Slot_Type) System.Enum.Parse(typeof(Equipment_Slot_Type),equipment_SlotText);
    public int Key => itemNumber;
    public List<ItemEffect> ItemEffects => itemEffects;
    public string ItemName => itemName;
    public string DescriptionText => descriptionText;
    public string ItemIconSourceText => itemIconSourceText;
    public Dictionary<string, Sprite> ImageSource { get => _imageSource; set => _imageSource = value; }

    public string GetItemEffectText()
    {
        string descriptionText = "";
        descriptionText = Utill.ItemGradeConvertToKorean(Item_Grade) + "µî±Þ\n";
        foreach (ItemEffect effect in ItemEffects)
        {
            descriptionText += $"{Utill.StatTypeConvertToKorean(effect.statType)} : {effect.value} \n";
        }

        return descriptionText;
    }

    public UI_ItemComponent_Inventory MakeItemComponentInventory(Transform parent = null, int itemCount = 1, string name = null, string path = null)
    {
        UI_ItemComponent_Equipment ui_Equipment_Component = Managers.UI_Manager.MakeSubItem<UI_ItemComponent_Equipment>(parent, name, $"Prefabs/UI/Item/UI_ItemComponent_Equipment");
        if (itemCount != 1)
        {
            Debug.LogWarning("Equipment items are uncountable.");
        }
        ui_Equipment_Component.IntializeItem(this);
        return ui_Equipment_Component;
    }

    public UI_ShopItemComponent MakeShopItemComponent(int itemPrice, Transform parent = null, int itemCount = 1, string name = null, string path = null)
    {
        UI_ShopItemComponent ui_shopItemComponent = Managers.UI_Manager.MakeSubItem<UI_ShopItemComponent>(parent, name, $"Prefabs/UI/Item/UI_ShopItemComponent");
        if (itemCount != 1)
        {
            Debug.LogWarning("Equipment items are uncountable.");
        }
        ui_shopItemComponent.IntializeItem(this, itemCount, itemPrice);
        return ui_shopItemComponent;
    }
}

