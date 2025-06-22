using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using Data.Item;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Stats;
using TMPro;
using UI.Popup.PopupUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.SubItem
{
    public class UIShopItemComponent : UIItemComponent
    {
        [Inject] IDestroyObject _destroyer;
        [Inject] private ItemDataManager _itemDataManager;
        [Inject] GameManagerEx _gameManagerEx;
        
        
        enum ItemICons
        {
            ItemIconImage,
            ItemGradeBorderImage
        }

        enum ItemTexts
        {
            ItemNameText,
            ItemPriceText,
            ItemCountText
        }

        private UIShop _uiShop;

        private TMP_Text _itemNameText;
        private TMP_Text _itemgPriceText;
        private TMP_Text _itemCountText;
        private int _itemPrice;
        private int _itemCount;
        private UIPlayerInventory _uiPlayerInventory;
        [Inject] private UIManager _uiManager;

        
        
        private RectTransform _itemRectTr;
        private PlayerStats _playerStats;
        private Image _itemGradeBorderImage;

        protected GraphicRaycaster _uiRaycaster;
        protected EventSystem _eventSystem;

        public int ItemCount
        {
            get => _itemCount;
            set
            {
                _itemCount = value;
                _itemCountText.text = _itemCount.ToString();
            }
        }
        public override RectTransform ItemRectTr => _itemRectTr;

        public int ItemPrice
        {
            get => _itemPrice;
            set
            {
                _itemPrice = value;
                _itemgPriceText.text = _itemPrice.ToString();
            }
        }


        protected override void AwakeInit()
        {
            Bind<Image>(typeof(ItemICons));
            Bind<TMP_Text>(typeof(ItemTexts));
            _itemIconSourceImage = GetImage((int)ItemICons.ItemIconImage);
            _itemGradeBorderImage = GetImage((int)ItemICons.ItemGradeBorderImage);
            _itemNameText = Get<TMP_Text>((int)ItemTexts.ItemNameText);
            _itemCountText = Get<TMP_Text>((int)ItemTexts.ItemCountText);
            _itemgPriceText = Get<TMP_Text>((int)ItemTexts.ItemPriceText);
            _itemRectTr = GetComponent<RectTransform>();
        }

        protected override void StartInit()
        {
            base.StartInit();
            _itemNameText.text = _itemName;
            _playerStats = _gameManagerEx.Player.GetComponent<PlayerStats>();
            _uiPlayerInventory = _uiManager.GetImportant_Popup_UI<UIPlayerInventory>();
            _uiShop = _uiManager.GetImportant_Popup_UI<UIShop>();
            _itemGradeBorderImage.sprite = _itemDataManager.ItemGradeBorder[ItemGradeType];


            _uiRaycaster = _uiShop.UIShopRayCaster;
            _eventSystem = _uiShop.EventSystem;
        }


        public override void ItemRightClick(PointerEventData eventdata)
        {
            base.ItemRightClick(eventdata);
            //아이템창이 닫힌상태에서 받는게 불가능하니, 닫힌상태에서는 루트아이템으로 보내버리기
            BuyItem();
        }

        private void BuyItem()
        {
            if (_playerStats.Gold < _itemPrice)
                return;

            _playerStats.Gold -= _itemPrice;//플레이어의 현재 돈을 깎는다.
            _iteminfo.MakeInventoryItemComponent(_uiManager);

            ItemCount--;
            if (_itemCount <= 0)
                _destroyer.DestroyObject(gameObject);
        }

        public override void GetDragEnd(PointerEventData eventData)
        {
            //인벤토리에 넣으면 아이템 사기
            _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
            _isDragging = false;
            List<RaycastResult> uiResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, uiResults);
            foreach (RaycastResult uiResult in uiResults)
            {
                if (uiResult.gameObject.TryGetComponentInChildren(out InventoryContentCoordinate contextTr))
                {
                    BuyItem();
                }
            }
            UIDragImageIcon.SetItemImageDisable();
        }

        public void InitializeItem(IItem iteminfo, int count, int price)
        {
            base.IntializeItem(iteminfo);
            if (iteminfo is ItemEquipment)
            {
                _itemCount = 1;
            }
            else if (iteminfo is ItemConsumable)
            {
                ItemCount += count;
            }
            ItemPrice = price;
        }
    }
}
