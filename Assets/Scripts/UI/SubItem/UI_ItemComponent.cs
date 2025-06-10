using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public struct IteminfoStruct : INetworkSerializable//TODO: 나중에 아이템 강화등 고유아이템의 능력치가 변화할때 이걸로 던질것
{
    public int ItemNumber;
    public ItemType Item_Type;
    public Item_Grade_Type Item_Grade_Type;
    public List<StatEffect> ItemEffects;
    public string ItemName;
    public string DescriptionText;
    public string ItemIconSourceText;
    public List<string> ItemSourcePath;


    public IteminfoStruct(IItem iitem)
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
            // 1. List의 개수 직렬화
            int count = ItemEffects == null ? 0 : ItemEffects.Count;
            serializer.SerializeValue(ref count);

            // 2. 원소를 하나씩 직렬화
            if (ItemEffects != null)
            {
                for (int i = 0; i < count; i++)
                {
                    StatEffect stateffect = ItemEffects[i];
                    serializer.SerializeValue(ref stateffect.value);
                    serializer.SerializeValue(ref stateffect.statType);
                    string buffname = stateffect.buffname == null ? "" : stateffect.buffname;
                    serializer.SerializeValue(ref buffname);
                }
            }
        }
        else
        {
            // 1. 수신 측에서 개수 역직렬화
            int count = 0;
            serializer.SerializeValue(ref count);

            // 2. List를 재생성 후 원소 채우기
            ItemEffects = new List<StatEffect>(count);
            for (int i = 0; i < count; i++)
            {
                StatEffect stat = default(StatEffect);
                serializer.SerializeValue(ref stat.value);
                serializer.SerializeValue(ref stat.statType);
                serializer.SerializeValue(ref stat.buffname);

                ItemEffects.Add(stat);
            }
        }

        if (serializer.IsWriter)
        {
            // 1. 개수 먼저 직렬화
            int pathCount = ItemSourcePath == null ? 0 : ItemSourcePath.Count;
            serializer.SerializeValue(ref pathCount);

            // 2. 원소(문자열) 하나씩 직렬화
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
            // 1. 개수 역직렬화
            int pathCount = 0;
            serializer.SerializeValue(ref pathCount);

            // 2. List<string> 재생성 후 읽기
            ItemSourcePath = new List<string>(pathCount);
            for (int i = 0; i < pathCount; i++)
            {
                string path = string.Empty; // 기본값
                serializer.SerializeValue(ref path);
                ItemSourcePath.Add(path);
            }
        }
    }
}


public abstract class UI_ItemComponent : UI_Base, IItem
{
    private readonly float ItemVisibleValue = 0.5f;

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
    protected UI_ItemDragImage _UI_dragImageIcon;
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
    public UI_ItemDragImage UI_DragImageIcon
    {
        get
        {
            if (_UI_dragImageIcon == null)
            {
                if (Managers.UI_Manager.UI_sceneDict.TryGetValue(typeof(UI_ItemDragImage), out UI_Scene itemDragIamge) == true)
                {
                    _UI_dragImageIcon = itemDragIamge as UI_ItemDragImage;
                }   
            }
            return _UI_dragImageIcon;
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
        _decriptionObject.UI_DescriptionEnable();

        _decriptionObject.DescriptionWindow.transform.position
            = _decriptionObject.SetDecriptionPos(transform, ItemRectTr.rect.width, ItemRectTr.rect.height);

        _decriptionObject.SetItemEffectText((_iteminfo as IItemDescriptionForm).GetItemEffectText());
        _decriptionObject.SetValue(_iteminfo);//여기에 부모클래스인 IITem이 나와야함
        _decriptionObject.SetDescription(_descriptionText);
    }
    protected override void OnDisableInit()
    {
        base.OnDisableInit();

        if (UI_DragImageIcon == null)
            return;

        if (UI_DragImageIcon.IsDragImageActive == true)//드래그 이미지가 살아있을 상점이나, 인벤토리가 닫힐때
        {
            UI_DragImageIcon.SetItemImageDisable();
        }
        RevertImage();
    }

    protected void RevertImage()
    {
        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
        _isDragging = false;
        UI_DragImageIcon.SetItemImageDisable();
    }


    public void CloseDescription(PointerEventData eventdata)
    {
        CloseDescription();
    }

    protected void CloseDescription()
    {
        _decriptionObject.UI_DescriptionDisable();
        _decriptionObject.SetdecriptionOriginPos();
    }

    public virtual void ItemRightClick(PointerEventData eventdata)
    {
        if (_decriptionObject.gameObject.activeSelf)
        {
            _decriptionObject.UI_DescriptionDisable();  
        }
    }

    public void GetDragBegin(PointerEventData eventData)
    {

        UI_DragImageIcon.SetImageSprite(_itemIconSourceImage.sprite);
        UI_DragImageIcon.SetItemImageEnable();
        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 0f);
        UI_DragImageIcon.SetImageSpriteColorAlpah(ItemVisibleValue);
        _isDragging = true;
    }

    public void DraggingItem(PointerEventData eventData)
    {
        UI_DragImageIcon.SetDragImagePosition(eventData.position);
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
        _iteminfo = iteminfo;//다른 클래스들이 형변환을 쉽게 하기 위해 인터페이스를 저장
    }


    public virtual void SetINewteminfo(IteminfoStruct iteminfo)
    {
        _itemNumber = iteminfo.ItemNumber;
        _itemType = iteminfo.Item_Type;
        _item_Grade = iteminfo.Item_Grade_Type;
        _Itemeffects = iteminfo.ItemEffects;
        _itemName = iteminfo.ItemName;
        _descriptionText = iteminfo.DescriptionText;
    }
}
