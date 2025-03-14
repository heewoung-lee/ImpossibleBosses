using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public struct IItemStruct : INetworkSerializable
{
    public int ItemNumber;
    public ItemType Item_Type;
    public Item_Grade_Type Item_Grade_Type;
    public List<StatEffect> ItemEffects;
    public string ItemName;
    public string DescriptionText;
    public string ItemIconSourceText;
    public List<string> ItemSourcePath;


    public IItemStruct(IItem iitem)
    {
        ItemNumber = iitem.ItemNumber;
        Item_Type = iitem.Item_Type;
        Item_Grade_Type = iitem.Item_Grade;
        ItemEffects = iitem.ItemEffects;
        ItemName = iitem.ItemName;
        DescriptionText = iitem.DescriptionText;
        ItemIconSourceText = iitem.ItemIconSourceText;
        ItemSourcePath = iitem.ImageSource.Select((itemSource)=> itemSource.Key).ToList();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ItemNumber);
        serializer.SerializeValue(ref Item_Type);
        serializer.SerializeValue(ref Item_Grade_Type);
        serializer.SerializeValue(ref ItemName);
        serializer.SerializeValue(ref DescriptionText);
        serializer.SerializeValue(ref ItemIconSourceText);
        if (serializer.IsWriter)
        {
            // 1. List�� ���� ����ȭ
            int count = ItemEffects == null ? 0 : ItemEffects.Count;
            serializer.SerializeValue(ref count);

            // 2. ���Ҹ� �ϳ��� ����ȭ
            if (ItemEffects != null)
            {
                for (int i = 0; i < count; i++)
                {
                    StatEffect stateffect = ItemEffects[i];
                    serializer.SerializeValue(ref stateffect.value);
                    serializer.SerializeValue(ref stateffect.statType);
                    serializer.SerializeValue(ref stateffect.buffname);
                }
            }
        }
        else
        {
            // 1. ���� ������ ���� ������ȭ
            int count = 0;
            serializer.SerializeValue(ref count);

            // 2. List�� ����� �� ���� ä���
            ItemEffects = new List<StatEffect>(count);
            for (int i = 0; i < count; i++)
            {
                StatEffect stat = default(StatEffect);
                serializer.SerializeValue(ref stat.buffname);
                serializer.SerializeValue(ref stat.statType);
                serializer.SerializeValue(ref stat.value);

                ItemEffects.Add(stat);
            }
        }

        if (serializer.IsWriter)
        {
            // 1. ���� ���� ����ȭ
            int pathCount = ItemSourcePath == null ? 0 : ItemSourcePath.Count;
            serializer.SerializeValue(ref pathCount);

            // 2. ����(���ڿ�) �ϳ��� ����ȭ
            if (ItemSourcePath != null)
            {
                for (int i = 0; i < pathCount; i++)
                {
                    string path = ItemSourcePath[i];
                    serializer.SerializeValue(ref path);
                }
            }
        }
        else
        {
            // 1. ���� ������ȭ
            int pathCount = 0;
            serializer.SerializeValue(ref pathCount);

            // 2. List<string> ����� �� �б�
            ItemSourcePath = new List<string>(pathCount);
            for (int i = 0; i < pathCount; i++)
            {
                string path = string.Empty; // �⺻��
                serializer.SerializeValue(ref path);
                ItemSourcePath.Add(path);
            }
        }
    }
}


public abstract class UI_ItemComponent : UI_Base, IItem
{
    protected IItem _iteminfo;
    protected int _itemNumber;
    protected ItemType _itemType;
    protected Item_Grade_Type _item_Grade;
    protected List<StatEffect> _Itemeffects;
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
    public List<StatEffect> ItemEffects => _Itemeffects;
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
