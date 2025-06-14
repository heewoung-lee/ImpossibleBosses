using System;
using System.Collections.Generic;
using System.Text;
using Data.DataType.ItemType.Interface;
using GameManagers;
using UI.SubItem;
using UnityEngine;
using Util;

namespace Data.DataType.ItemType
{
    [Serializable]
    public class ItemConsumable : IKey<int>, IItem, IInventoryItemMaker, IItemDescriptionForm, IShopItemMaker
    {
        public int itemNumber;
        public Interface.ItemType itemType = Interface.ItemType.Consumable;
        public string itemGradeText = "Normal";
        public List<StatEffect> itemEffects = new List<StatEffect>();
        public string itemName;
        public string descriptionText;
        public string itemIconSourceText;
        public float duration = 0f;

        private Dictionary<string, Sprite> _imageSource = new Dictionary<string, Sprite>();

        public int ItemNumber => itemNumber;
        public int Key => itemNumber;
        public Interface.ItemType ItemType => itemType;
        public ItemGradeType ItemGradeType => (ItemGradeType)System.Enum.Parse(typeof(ItemGradeType), itemGradeText);
        public List<StatEffect> ItemEffects => itemEffects;
        public string ItemName => itemName;
        public string DescriptionText => descriptionText;
        public string ItemIconSourceText => itemIconSourceText;
        public float Duration => duration;

        public Dictionary<string, Sprite> ImageSource
        {
            get => _imageSource;
            set => _imageSource = value;
        }

        public ItemConsumable()
        {
        }

        public ItemConsumable(IItem iteminfo)
        {
            itemNumber = iteminfo.ItemNumber;
            itemType = Interface.ItemType.Consumable;
            itemGradeText = iteminfo.ItemGradeType.ToString();
            itemEffects = iteminfo.ItemEffects;
            itemName = iteminfo.ItemName;
            descriptionText = iteminfo.DescriptionText;
            itemIconSourceText = iteminfo.ItemIconSourceText;
            duration = ((ItemConsumable)iteminfo).Duration;
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
                descriptionBuilder.AppendLine(
                    $"{Utill.StatTypeConvertToKorean(effect.statType)} {effect.value} {actionText}");
            }

            // 지속시간 정보 추가
            if (duration > 0)
            {
                descriptionBuilder.AppendLine($"지속시간: {duration}초");
            }

            return descriptionBuilder.ToString();
        }

        public UIItemComponentInventory MakeItemComponentInventory(Transform parent = null, int itemCount = 1,
            string name = null, string path = null)
        {
            UIItemComponentConsumable uiConsumableComponent =
                Managers.UIManager.MakeSubItem<UIItemComponentConsumable>(parent, name,
                    $"Prefabs/UI/Item/UI_ItemComponent_Consumable");
            uiConsumableComponent.InitializeItem(this, itemCount);
            return uiConsumableComponent;
        }

        public UIShopItemComponent MakeShopItemComponent(int itemPrice, Transform parent = null, int itemCount = 1,
            string name = null, string path = null)
        {
            UIShopItemComponent uiShopItemComponent =
                Managers.UIManager.MakeSubItem<UIShopItemComponent>(parent, name,
                    $"Prefabs/UI/Item/UIShopItemComponent");
            uiShopItemComponent.InitializeItem(this, itemCount, itemPrice);
            return uiShopItemComponent;
        }
    }
}