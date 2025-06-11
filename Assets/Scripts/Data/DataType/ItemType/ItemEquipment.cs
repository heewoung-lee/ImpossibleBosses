using System;
using System.Collections.Generic;
using Data.DataType.ItemType.Interface;
using UnityEngine;

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

        public UI_ItemComponent_Inventory MakeItemComponentInventory(Transform parent = null, int itemCount = 1,
            string name = null, string path = null)
        {
            UI_ItemComponent_Equipment uiEquipmentComponent
                = Managers.UI_Manager.MakeSubItem<UI_ItemComponent_Equipment>(parent, name,
                    $"Prefabs/UI/Item/UI_ItemComponent_Equipment");
            if (itemCount != 1)
            {
                Debug.LogWarning("Equipment items are uncountable.");
            }

            uiEquipmentComponent.IntializeItem(this);
            return uiEquipmentComponent;
        }

        public UI_ShopItemComponent MakeShopItemComponent(int itemPrice, Transform parent = null, int itemCount = 1,
            string name = null, string path = null)
        {
            UI_ShopItemComponent uiShopItemComponent =
                Managers.UI_Manager.MakeSubItem<UI_ShopItemComponent>(parent, name,
                    $"Prefabs/UI/Item/UI_ShopItemComponent");
            if (itemCount != 1)
            {
                Debug.LogWarning("Equipment items are uncountable.");
            }

            uiShopItemComponent.IntializeItem(this, itemCount, itemPrice);
            return uiShopItemComponent;
        }
    }
}