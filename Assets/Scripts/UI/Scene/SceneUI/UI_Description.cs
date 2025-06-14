using System;
using System.Collections;
using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using GameManagers;
using TMPro;
using UI.Popup.PopupUI;
using UI.Scene;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class UI_Description : UIScene
{
    enum ImageType
    {
        ItemImage
    }

    enum ItemDescription
    {
        ItemNameText,
        ItemType,
        ItemGradeType,
        ItemEffectText,
        DescriptionText,
    }


    enum Direction
    {
        Right,
        Down,
        Left,
        Up,
    }

    private Image _itemImage;
    private TMP_Text _itemNameText;
    private TMP_Text _itemType;
    private TMP_Text _itemEffectText;
    private TMP_Text _itemDescriptionText;
    private DescriptionWindow _descriptionWindow;
    private RectTransform _description_RectTr;
    private float _descriptionWidth;
    private float _descriptionHeight;
    private Canvas _descriptionCanvas;
    private Direction _currnt_Dir = Direction.Right;
    private Color _item_GradeColor;
    private Vector3 _originPos;
    private UIPlayerInventory _uI_Player_Inventory;


    public DescriptionWindow DescriptionWindow => _descriptionWindow;


    public UIPlayerInventory UI_Player_Inventory
    {
        get
        {
            if(_uI_Player_Inventory == null)
            {
                _uI_Player_Inventory = Managers.UIManager.GetImportant_Popup_UI<UIPlayerInventory>();
            }
            return _uI_Player_Inventory;
        }
    }

    public Vector3 OriginPos { get => _originPos; }
    public bool isDescriptionActive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }


    protected override void AwakeInit()
    {
        Bind<Image>(typeof(ImageType));
        Bind<TMP_Text>(typeof(ItemDescription));

        _itemImage = Get<Image>((int)ImageType.ItemImage);
        _itemNameText = Get<TMP_Text>((int)ItemDescription.ItemNameText);
        _itemType = Get<TMP_Text>((int)ItemDescription.ItemType);
        _itemEffectText = Get<TMP_Text>((int)ItemDescription.ItemEffectText);
        _itemDescriptionText =Get<TMP_Text>((int)ItemDescription.DescriptionText);
        _originPos = transform.position;

        _descriptionWindow = GetComponentInChildren<DescriptionWindow>();
        _description_RectTr = _descriptionWindow.GetComponent<RectTransform>();
        _descriptionWidth = _description_RectTr.rect.width;
        _descriptionHeight = _description_RectTr.rect.height;
        _descriptionCanvas = GetComponent<Canvas>();

        gameObject.SetActive(false);
    }

    public void UI_DescriptionEnable()
    {
        gameObject.SetActive(true);
    }


    public void UI_DescriptionDisable()
    {
        gameObject.SetActive(false);
    }

    public void SetValue(IItem iteminfo)
    {
        _item_GradeColor = Utill.GetItemGradeColor(iteminfo.ItemGradeType);
        _itemImage.sprite = iteminfo.ImageSource[iteminfo.ItemIconSourceText];
        _itemNameText.text = iteminfo.ItemName;
        _itemNameText.color = _itemNameText.SetGradeColor(_item_GradeColor);
        _itemType.text = GetItemType(iteminfo);
        //아이템 타입으로 스위치를 나눠서
        //장비 아이템이면 장비아이템 타입으로 나눔
        //소비 아이템이면 타입에 소비아이템으로 나눔
    }

    public void SetValue(Sprite iconSprite,string name,string typeText = null)
    {
        _itemImage.sprite = iconSprite;
        _itemNameText.text = name;
        _itemType.text = typeText;
    }
    public void SetItemEffectText(string text)
    {
        _itemEffectText.text = text;
    }

    public void SetDescription(string text)
    {
        _itemDescriptionText.text = text;
    }

    public string GetItemType(IItem iteminfo)
    {
        switch (iteminfo.ItemType)
        {
            case ItemType.Equipment:
                ItemEquipment equip = iteminfo as ItemEquipment;
                return ConvertEquipItemTypeToKorean(equip.EquipmentSlotType);
            case ItemType.Consumable:
                return "소비아이템";
            case ItemType.ETC:
                return "기타아이템";
        }
        return "기타아이템";
    }
    public string ConvertEquipItemTypeToKorean(EquipmentSlotType EquipType)
    {
        switch (EquipType)
        {
            case EquipmentSlotType.Helmet:
                return "머리";
            case EquipmentSlotType.Gauntlet:
                return "장갑";
            case EquipmentSlotType.Shoes:
                return "신발";
            case EquipmentSlotType.Weapon:
                return "무기";
            case EquipmentSlotType.Ring:
                return "반지";
            case EquipmentSlotType.Armor:
                return "갑옷";
        }

        return "알수없는장비";
    }

    protected override void StartInit()
    {
        _descriptionCanvas.sortingOrder = (int)Define.SpecialSortingOrder.Description;
        
    }
    private void OnEnable()
    {
        _descriptionCanvas.sortingOrder = UI_Player_Inventory.GetComponent<Canvas>().sortingOrder + 1;
    }

    public Vector3 SetDecriptionPos(Transform componentTr, float width, float height)
    {
        _currnt_Dir = Direction.Right;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, componentTr.position);
        //다른 캔버스의 UI위치를 다른캔버스로 옮겨야 하기 때문에 먼저 해당 로컬좌표를 화면좌표로 변환한다.
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _description_RectTr,
            screenPoint,
            null,
            out localPoint
        );//변환한 화면좌표를 UI_Description의 로컬포지션으로 변경한다.

        Vector3 setPos = localPoint;
        int direction_settingCount = 0;

        float screenWidth = Screen.width; // 화면의 너비
        float screenHeight = Screen.height; // 화면의 높이
        while (true)
        {
            direction_settingCount++;
            if (direction_settingCount > 4)
            {
                _description_RectTr.pivot = new Vector2(0.5f, 0.5f);
                return OriginPos;
            }

            switch (_currnt_Dir)
            {
                case Direction.Right:
                    _description_RectTr.pivot = new Vector2(0, 1);//왼쪽 상단
                    setPos = new Vector2(localPoint.x + width / 2, localPoint.y + height / 2);
                    break;
                case Direction.Down:
                    _description_RectTr.pivot = new Vector2(0.5f, 1);//중앙 상단
                    setPos = new Vector2(localPoint.x + width / 2, localPoint.y - height / 2);
                    break;
                case Direction.Left:
                    _description_RectTr.pivot = new Vector2(1, 1);//오른쪽 상단
                    setPos = new Vector2(localPoint.x - width / 2, localPoint.y + height / 2);
                    break;
                case Direction.Up:
                    _description_RectTr.pivot = new Vector2(0.5f, 0);//중앙 하단
                    setPos = new Vector2(localPoint.x + width / 2, localPoint.y + height / 2);
                    break;
            }

            Vector3 worldPos = _description_RectTr.TransformPoint(setPos);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);//다시 스크린좌표로 변환해서 아이템설명창이 화면밖으로 나가는지 확인

            float minPositionX = screenPos.x + (_description_RectTr.pivot.x * -1 * _description_RectTr.rect.width);// setPos.x; 그대로
            float maxPositionX = screenPos.x + (_description_RectTr.pivot.x * -1 + 1) * _description_RectTr.rect.width;// setPos.x+ width
            float minPositionY = screenPos.y + (_description_RectTr.pivot.y * -1 * _description_RectTr.rect.height);
            float maxPositionY = screenPos.y + (_description_RectTr.pivot.y * -1 + 1) * _description_RectTr.rect.height;


            if (minPositionX > 0 && maxPositionX < screenWidth && minPositionY > 0 && minPositionY < screenHeight)
            {
                return screenPos;
            }
            else
            {
                _currnt_Dir = (Direction)(((int)_currnt_Dir + 1) % Enum.GetValues(typeof(Direction)).Length);
                continue;
            }
        }
    }
    public void SetdecriptionOriginPos()
    {
        transform.position = OriginPos;
    }
}