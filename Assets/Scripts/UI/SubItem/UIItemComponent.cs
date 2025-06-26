using System.Collections.Generic;
using System.Linq;
using Data.DataType.ItemType.Interface;
using GameManagers;
using GameManagers.Interface.UIManager;
using UI.Scene;
using UI.Scene.SceneUI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.SubItem
{
    public struct IteminfoStruct : INetworkSerializable//TODO: 나중에 아이템 강화등 고유아이템의 능력치가 변화할때 이걸로 던질것
    {
        public int ItemNumber;
        public ItemType ItemType;
        public ItemGradeType ItemGradeType;
        public List<StatEffect> ItemEffects;
        public string ItemName;
        public string DescriptionText;
        public string ItemIconSourceText;
        public List<string> ItemSourcePath;


        public IteminfoStruct(IItem iitem)
        {
            ItemNumber = iitem.ItemNumber;
            ItemType = iitem.ItemType;
            ItemGradeType = iitem.ItemGradeType;
            ItemEffects = iitem.ItemEffects;
            ItemName = iitem.ItemName;
            DescriptionText = iitem.DescriptionText;
            ItemIconSourceText = iitem.ItemIconSourceText;
            ItemSourcePath = iitem.ImageSource.Select((itemSource)=> itemSource.Key).ToList();
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ItemNumber);
            serializer.SerializeValue(ref ItemType);
            serializer.SerializeValue(ref ItemGradeType);
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


    public abstract class UIItemComponent : UIBase, IItem
    {
        private readonly float _itemVisibleValue = 0.5f;
        [Inject] private IUISceneManager _uiSceneManager;

        protected IItem _iteminfo;
        protected int _itemNumber;
        protected ItemType _itemType;
        protected ItemGradeType _itemGrade;
        protected List<StatEffect> _Itemeffects;
        protected string _itemName;
        protected string _descriptionText;
        protected string _itemIconSourceImageText;
        protected Image _itemIconSourceImage;
        protected Dictionary<string, Sprite> _imageSource;
        protected UI_ItemDragImage _ui_dragImageIcon;
        protected UIDescription _decriptionObject;
        protected bool _isDragging = false;

        public int ItemNumber => _itemNumber;
        public ItemType ItemType => _itemType;
        public ItemGradeType ItemGradeType => _itemGrade;
        public List<StatEffect> ItemEffects => _Itemeffects;
        public string ItemName => _itemName;
        public string DescriptionText => _descriptionText;
        public string ItemIconSourceText => _itemIconSourceImageText;
        public Dictionary<string, Sprite> ImageSource => _imageSource;

        public abstract RectTransform ItemRectTr { get; }
        public UI_ItemDragImage UIDragImageIcon
        {
            get
            {
                if (_ui_dragImageIcon == null)
                {
                    if (_uiSceneManager.Try_Get_Scene_UI<UI_ItemDragImage>(out UI_ItemDragImage itemDragIamge) == true)
                    {
                        _ui_dragImageIcon = itemDragIamge;
                    }   
                }
                return _ui_dragImageIcon;
            }

        }


        protected override void StartInit()
        {

           

            _decriptionObject = _uiSceneManager.Get_Scene_UI<UIDescription>();
        }


        public void ShowDescription(PointerEventData eventdata)
        {
            if (_isDragging)
                return;
            _decriptionObject.UI_DescriptionEnable();

            _decriptionObject.DescriptionWindow.transform.position
                = _decriptionObject.SetDecriptionPos(transform, ItemRectTr.rect.width, ItemRectTr.rect.height);

            _decriptionObject.SetItemEffectText(((IItemDescriptionForm)_iteminfo).GetItemEffectText());
            _decriptionObject.SetValue(_iteminfo);//여기에 부모클래스인 IITem이 나와야함
            _decriptionObject.SetDescription(_descriptionText);
        }
        protected override void OnEnableInit()
        {
            base.OnEnableInit();
            BindEvent(gameObject, ShowDescription, Define.UIEvent.PointerEnter);
            BindEvent(gameObject, CloseDescription, Define.UIEvent.PointerExit);
            BindEvent(gameObject, ItemRightClick, Define.UIEvent.RightClick);
            BindEvent(gameObject, GetDragBegin, Define.UIEvent.DragBegin);
            BindEvent(gameObject, DraggingItem, Define.UIEvent.Drag);
            BindEvent(gameObject, GetDragEnd, Define.UIEvent.DragEnd);
        }
        protected override void OnDisableInit()
        {
            base.OnDisableInit();

            if (UIDragImageIcon == null)
                return;

            if (UIDragImageIcon.IsDragImageActive == true)//드래그 이미지가 살아있을 상점이나, 인벤토리가 닫힐때
            {
                UIDragImageIcon.SetItemImageDisable();
            }
            UnBindEvent(gameObject, ShowDescription, Define.UIEvent.PointerEnter);
            UnBindEvent(gameObject, CloseDescription, Define.UIEvent.PointerExit);
            UnBindEvent(gameObject, ItemRightClick, Define.UIEvent.RightClick);
            UnBindEvent(gameObject, GetDragBegin, Define.UIEvent.DragBegin);
            UnBindEvent(gameObject, DraggingItem, Define.UIEvent.Drag);
            UnBindEvent(gameObject, GetDragEnd, Define.UIEvent.DragEnd);
            RevertImage();
        }

        protected void RevertImage()
        {
            _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
            _isDragging = false;
            UIDragImageIcon.SetItemImageDisable();
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

            UIDragImageIcon.SetImageSprite(_itemIconSourceImage.sprite);
            UIDragImageIcon.SetItemImageEnable();
            _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 0f);
            UIDragImageIcon.SetImageSpriteColorAlpah(_itemVisibleValue);
            _isDragging = true;
        }

        public void DraggingItem(PointerEventData eventData)
        {
            UIDragImageIcon.SetDragImagePosition(eventData.position);
        }


        public abstract void GetDragEnd(PointerEventData eventData);


        public virtual void IntializeItem(IItem iteminfo)
        {
            _itemNumber = iteminfo.ItemNumber;
            _itemType = iteminfo.ItemType;
            _itemGrade = iteminfo.ItemGradeType;
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
            _itemType = iteminfo.ItemType;
            _itemGrade = iteminfo.ItemGradeType;
            _Itemeffects = iteminfo.ItemEffects;
            _itemName = iteminfo.ItemName;
            _descriptionText = iteminfo.DescriptionText;
        }
    }
}