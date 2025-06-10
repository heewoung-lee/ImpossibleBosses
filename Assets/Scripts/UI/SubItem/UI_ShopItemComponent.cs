using System.Collections;
using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using Data.Item;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopItemComponent : UI_ItemComponent
{

    enum ItemICons
    {
        ItemIcon_Image,
        ItemGradeBorder_Image
    }

    enum ItemTexts
    {
        ItemName_Text,
        Item_Price_Text,
        ItemCount_Text
    }

    private UI_Shop _ui_Shop;

    private TMP_Text _itemName_Text;
    private TMP_Text _itemgPrice_Text;
    private TMP_Text _itemCount_Text;
    private int _itemPrice;
    private int _itemCount;

    private RectTransform _itemRectTr;
    private UI_Player_Inventory _ui_Player_Inventory;
    private PlayerStats _playerStats;
    private Image _itemGradeBorder_Image;

    protected GraphicRaycaster _uiRaycaster;
    protected EventSystem _eventSystem;

    public int ItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            _itemCount_Text.text = _itemCount.ToString();
        }
    }
    public override RectTransform ItemRectTr => _itemRectTr;

    public int ItemPrice
    {
        get => _itemPrice;
        set
        {
            _itemPrice = value;
            _itemgPrice_Text.text = _itemPrice.ToString();
        }
    }


    protected override void AwakeInit()
    {
        Bind<Image>(typeof(ItemICons));
        Bind<TMP_Text>(typeof(ItemTexts));
        _itemIconSourceImage = GetImage((int)ItemICons.ItemIcon_Image);
        _itemGradeBorder_Image = GetImage((int)ItemICons.ItemGradeBorder_Image);
        _itemName_Text = Get<TMP_Text>((int)ItemTexts.ItemName_Text);
        _itemCount_Text = Get<TMP_Text>((int)ItemTexts.ItemCount_Text);
        _itemgPrice_Text = Get<TMP_Text>((int)ItemTexts.Item_Price_Text);
        _itemRectTr = GetComponent<RectTransform>();
    }

    protected override void StartInit()
    {
        base.StartInit();
        _itemName_Text.text = _itemName;
        _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
        _ui_Player_Inventory = Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>();
        _ui_Shop = Managers.UI_Manager.GetImportant_Popup_UI<UI_Shop>();
        _itemGradeBorder_Image.sprite = Managers.ItemDataManager.ItemGradeBorder[ItemGradeType];


        _uiRaycaster = _ui_Shop.UI_Shop_RayCaster;
        _eventSystem = _ui_Shop.EventSystem;
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
        _iteminfo.MakeInventoryItemComponent();

        ItemCount--;
        if (_itemCount <= 0)
            Managers.ResourceManager.DestroyObject(gameObject);
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
        UI_DragImageIcon.SetItemImageDisable();
    }

    public void IntializeItem(IItem iteminfo, int count, int price)
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
