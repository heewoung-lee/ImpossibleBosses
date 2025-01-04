using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class UI_ItemComponent : UI_Base, IItem
{
    protected IItem _iteminfo;
    protected int _itemNumber;
    protected ItemType _itemType;
    protected Item_Grade_Type _item_Grade;
    protected List<ItemEffect> _Itemeffects;
    protected string _itemName;
    protected string _descriptionText;
    protected string _itemIconSourceImageText;
    protected Image _itemIconSourceImage;
    protected Dictionary<string, Sprite> _imageSource;
    protected Image _dragImageIcon;
    protected UI_Description _decriptionObject;
    protected bool _isDragging = false;

    public int ItemNumber => _itemNumber;
    public ItemType Item_Type => _itemType;
    public Item_Grade_Type Item_Grade => _item_Grade;
    public List<ItemEffect> ItemEffects => _Itemeffects;
    public string ItemName => _itemName;
    public string DescriptionText => _descriptionText;
    public string ItemIconSourceText => _itemIconSourceImageText;
    public Dictionary<string, Sprite> ImageSource => _imageSource;

    public abstract RectTransform ItemRectTr { get; }
    public Image DragImageIcon
    {
        get
        {
            if (_dragImageIcon == null)
            {
                _dragImageIcon = (Managers.UI_Manager.UI_sceneDict[typeof(UI_ItemDragImage)] as UI_ItemDragImage).ItemDragImage;
            }
            return _dragImageIcon;
        }

    }


    protected override void StartInit()
    {

        BindEvent(gameObject, ShowDescription, Define.UI_Event.PointerEnter);
        BindEvent(gameObject, CloseDescription, Define.UI_Event.PointerExit);
        BindEvent(gameObject, ItemRightClick, Define.UI_Event.RightClick);
        BindEvent(gameObject, GetDragBegin, Define.UI_Event.DragBegin);
        BindEvent(gameObject, DraggingItem, Define.UI_Event.Drag);
        BindEvent(gameObject, GetDragEnd, Define.UI_Event.DragEnd);

        _decriptionObject = Managers.UI_Manager.Get_Scene_UI<UI_Description>();
    }

    public void ShowDescription(PointerEventData eventdata)
    {
        if (_isDragging)
            return;
        _decriptionObject.gameObject.SetActive(true);

        _decriptionObject.DescriptionWindow.transform.position
            = _decriptionObject.SetDecriptionPos(transform, ItemRectTr.rect.width, ItemRectTr.rect.height);

        _decriptionObject.SetItemEffectText((_iteminfo as IItemDescriptionForm).GetItemEffectText());
        _decriptionObject.SetValue(_iteminfo);//���⿡ �θ�Ŭ������ IITem�� ���;���
        _decriptionObject.SetDescription(_descriptionText);
    }
    protected override void OnDisableInit()
    {
        base.OnDisableInit();
        if (DragImageIcon.gameObject.activeSelf)//�巡�� �̹����� ��������� �����̳�, �κ��丮�� ������
        {
            DragImageIcon.gameObject.SetActive(false);
        }
        RevertImage();
    }

    protected void RevertImage()
    {
        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
        _isDragging = false;
        DragImageIcon.gameObject.SetActive(false);
    }


    public void CloseDescription(PointerEventData eventdata)
    {
        CloseDescription();
    }

    protected void CloseDescription()
    {
        _decriptionObject.gameObject.SetActive(false);
        _decriptionObject.SetdecriptionOriginPos();
    }

    public virtual void ItemRightClick(PointerEventData eventdata)
    {
        if (_decriptionObject.gameObject.activeSelf)
        {
            _decriptionObject.gameObject.SetActive(false);
        }
    }

    public void GetDragBegin(PointerEventData eventData)
    {

        DragImageIcon.sprite = _itemIconSourceImage.sprite;
        DragImageIcon.gameObject.SetActive(true);//�巡�� �� �̹���

        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 0f);
        DragImageIcon.color = new Color(DragImageIcon.color.r, DragImageIcon.color.g, DragImageIcon.color.b, 0.5f);

        _isDragging = true;
    }

    public void DraggingItem(PointerEventData eventData)
    {
        DragImageIcon.transform.position = eventData.position;
    }
    public abstract void GetDragEnd(PointerEventData eventData);


    public virtual void IntializeItem(IItem iteminfo)
    {
        _itemNumber = iteminfo.ItemNumber;
        _itemType = iteminfo.Item_Type;
        _item_Grade = iteminfo.Item_Grade;
        _Itemeffects = iteminfo.ItemEffects;
        _itemName = iteminfo.ItemName;
        _descriptionText = iteminfo.DescriptionText;
        _itemIconSourceImageText = iteminfo.ItemIconSourceText;
        _itemIconSourceImage.sprite = iteminfo.ImageSource[iteminfo.ItemIconSourceText];
        _imageSource = iteminfo.ImageSource;
        _iteminfo = iteminfo;//�ٸ� Ŭ�������� ����ȯ�� ���� �ϱ� ���� �������̽��� ����
    }

}
