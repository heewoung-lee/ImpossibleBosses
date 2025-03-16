using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[Serializable]
public class ItemConsumable : Ikey<int>, IItem, IInventoryItemMaker, IItemDescriptionForm, IShopItemMaker
{
    public int itemNumber;
    public ItemType itemType = ItemType.Consumable;
    public string itemGradeText = "Normal";
    public List<StatEffect> itemEffects = new List<StatEffect>();
    public string itemName;
    public string descriptionText;
    public string itemIconSourceText;
    public float duration = 0f;

    private Dictionary<string, Sprite> _imageSource = new Dictionary<string, Sprite>();

    public int ItemNumber => itemNumber;
    public int Key => itemNumber;
    public ItemType Item_Type => itemType;
    public Item_Grade_Type Item_Grade => (Item_Grade_Type)System.Enum.Parse(typeof(Item_Grade_Type), itemGradeText);
    public List<StatEffect> ItemEffects => itemEffects;
    public string ItemName => itemName;
    public string DescriptionText => descriptionText;
    public string ItemIconSourceText => itemIconSourceText;
    public float Duration => duration;
    public Dictionary<string, Sprite> ImageSource { get => _imageSource; set => _imageSource = value; }
    public ItemConsumable(){ }

    public ItemConsumable(IItem iteminfo)
    {
        itemNumber = iteminfo.ItemNumber;
        itemType = ItemType.Consumable;
        itemGradeText = iteminfo.Item_Grade.ToString();
        itemEffects = iteminfo.ItemEffects;
        itemName = iteminfo.ItemName;
        descriptionText = iteminfo.DescriptionText;
        itemIconSourceText = iteminfo.ItemIconSourceText;
        duration = (iteminfo as ItemConsumable).Duration;
    }

    public string GetItemEffectText()
    {
        StringBuilder descriptionBuilder = new StringBuilder();

        // 기본 설명 추가
        descriptionBuilder.AppendLine(DescriptionText);

        // 효과들에 대한 설명 추가
        foreach (StatEffect effect in ItemEffects)
        {
            string actionText = (duration > 0) ? "증가" : "회복";
            descriptionBuilder.AppendLine($"{Utill.StatTypeConvertToKorean(effect.statType)} {effect.value} {actionText}");
        }

        // 지속시간 정보 추가
        if (duration > 0)
        {
            descriptionBuilder.AppendLine($"지속시간: {duration}초");
        }

        return descriptionBuilder.ToString();
    }
    public UI_ItemComponent_Inventory MakeItemComponentInventory(Transform parent = null, int itemCount = 1, string name = null, string path = null)
    {
        UI_ItemComponent_Consumable ui_Conuable_Component= Managers.UI_Manager.MakeSubItem<UI_ItemComponent_Consumable>(parent, name, $"Prefabs/UI/Item/UI_ItemComponent_Consumable");
        ui_Conuable_Component.IntializeItem(this, itemCount);
        return ui_Conuable_Component;
    }

    public UI_ShopItemComponent MakeShopItemComponent(int itemPrice, Transform parent = null, int itemCount = 1, string name = null, string path = null)
    {
        UI_ShopItemComponent ui_shopItemComponent = Managers.UI_Manager.MakeSubItem<UI_ShopItemComponent>(parent, name, $"Prefabs/UI/Item/UI_ShopItemComponent");
        ui_shopItemComponent.IntializeItem(this, itemCount, itemPrice);
        return ui_shopItemComponent;
    }
}

