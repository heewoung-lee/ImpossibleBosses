using System;
using System.Collections.Generic;
using Data.DataType.ItemType.Interface;
using GameManagers;
using UI.SubItem;
using UnityEngine;
using Util;

namespace Data.DataType.ItemType
{
    public enum EquipmentSlotType
    {
        Helmet,
        Gauntlet,
        Shoes,
        Weapon,
        Ring,
        Armor
    }

    [Serializable]
    public class ItemEquipment : IKey<int>, IItem, IInventoryItemMaker, IItemDescriptionForm, IShopItemMaker
    {
        public int itemNumber;
        public Interface.ItemType itemType = Interface.ItemType.Equipment;
        public string itemGradeText;
        public string equipmentSlotText;
        public List<StatEffect> itemEffects = new List<StatEffect>();
        public string itemName;
        public string descriptionText;
        public string itemIconSourceText;

        public ItemEquipment()
        {
        }

        public ItemEquipment(IItem iteminfo)
        {
            itemNumber = iteminfo.ItemNumber;
            itemType = Interface.ItemType.Consumable;
            itemGradeText = iteminfo.ItemGradeType.ToString();
            itemEffects = iteminfo.ItemEffects;
            itemName = iteminfo.ItemName;
            descriptionText = iteminfo.DescriptionText;
            itemIconSourceText = iteminfo.ItemIconSourceText;
            equipmentSlotText = ((ItemEquipment)iteminfo).equipmentSlotText;
        }

        private Dictionary<string, Sprite> _imageSource = new Dictionary<string, Sprite>();
        public int ItemNumber => itemNumber;
        public Interface.ItemType ItemType => itemType;
        public ItemGradeType ItemGradeType => (ItemGradeType)Enum.Parse(typeof(ItemGradeType), itemGradeText);
        public EquipmentSlotType EquipmentSlotType =>(EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), equipmentSlotText);

        public int Key => itemNumber;
        public List<StatEffect> ItemEffects => itemEffects;
        public string ItemName => itemName;
        public string DescriptionText => descriptionText;
        public string ItemIconSourceText => itemIconSourceText;

        public Dictionary<string, Sprite> ImageSource
        {
            get => _imageSource;
            set => _imageSource = value;
        }

        public string GetItemEffectText()
        {
            string itemEffectText = "";
            itemEffectText = Utill.ItemGradeConvertToKorean(ItemGradeType) + "등급\n";
            foreach (StatEffect effect in ItemEffects)
            {
                itemEffectText += $"{Utill.StatTypeConvertToKorean(effect.statType)} : {effect.value} \n";
            }

            return itemEffectText;
        }

        public UIItemComponentInventory MakeItemComponentInventory(Transform parent = null, int itemCount = 1,
            string name = null, string path = null)
        {
            UIItemComponentEquipment uiEquipmentComponent
                = Managers.UIManager.MakeSubItem<UIItemComponentEquipment>(parent, name,
                    $"Prefabs/UI/Item/UI_ItemComponent_Equipment");
            if (itemCount != 1)
            {
                Debug.LogWarning("Equipment items are uncountable.");
            }

            uiEquipmentComponent.IntializeItem(this);
            return uiEquipmentComponent;
        }

        public UIShopItemComponent MakeShopItemComponent(int itemPrice, Transform parent = null, int itemCount = 1,
            string name = null, string path = null)
        {
            UIShopItemComponent uiShopItemComponent =
                Managers.UIManager.MakeSubItem<UIShopItemComponent>(parent, name,
                    $"Prefabs/UI/Item/UIShopItemComponent");
            if (itemCount != 1)
            {
                Debug.LogWarning("Equipment items are uncountable.");
            }

            uiShopItemComponent.InitializeItem(this, itemCount, itemPrice);
            return uiShopItemComponent;
        }
    }
}