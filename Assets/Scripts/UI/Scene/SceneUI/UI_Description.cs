using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Description : UI_Scene
{
    private const int UI_DESCRIPTION_SORTING_ORDER = 50;
    
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
    public DescriptionWindow DescriptionWindow => _descriptionWindow;

    private Color _item_GradeColor;
    private Vector3 _originPos;
    public Vector3 OriginPos { get => _originPos; }

    private Direction _currnt_Dir = Direction.Right;

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
    }



    public void SetValue(IItem iteminfo)
    {
        _item_GradeColor = Utill.GetItemGradeColor(iteminfo.Item_Grade);
        _itemImage.sprite = iteminfo.ImageSource[iteminfo.ItemIconSourceText];
        _itemNameText.text = iteminfo.ItemName;
        _itemNameText.color = _itemNameText.SetGradeColor(_item_GradeColor);
        _itemType.text = GetItemType(iteminfo);
        //������ Ÿ������ ����ġ�� ������
        //��� �������̸� �������� Ÿ������ ����
        //�Һ� �������̸� Ÿ�Կ� �Һ���������� ����
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
        switch (iteminfo.Item_Type)
        {
            case ItemType.Equipment:
                ItemEquipment equip = iteminfo as ItemEquipment;
                return ConvertEquipItemTypeToKorean(equip.Equipment_Slot);
            case ItemType.Consumable:
                return "�Һ������";
            case ItemType.ETC:
                return "��Ÿ������";
        }
        return "��Ÿ������";
    }
    public string ConvertEquipItemTypeToKorean(Equipment_Slot_Type EquipType)
    {
        switch (EquipType)
        {
            case Equipment_Slot_Type.Helmet:
                return "�Ӹ�";
            case Equipment_Slot_Type.Gauntlet:
                return "�尩";
            case Equipment_Slot_Type.Shoes:
                return "�Ź�";
            case Equipment_Slot_Type.Weapon:
                return "����";
            case Equipment_Slot_Type.Ring:
                return "����";
            case Equipment_Slot_Type.Armor:
                return "����";
        }

        return "�˼��������";
    }

    protected override void StartInit()
    {
        _descriptionCanvas.sortingOrder = UI_DESCRIPTION_SORTING_ORDER;
        gameObject.SetActive(false);
    }

    public Vector3 SetDecriptionPos(UI_ItemComponent itemComponent, float width, float height)
    {
        _currnt_Dir = Direction.Right;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, itemComponent.transform.position);
        //�ٸ� ĵ������ UI��ġ�� �ٸ�ĵ������ �Űܾ� �ϱ� ������ ���� �ش� ������ǥ�� ȭ����ǥ�� ��ȯ�Ѵ�.
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _description_RectTr,
            screenPoint,
            null,
            out localPoint
        );//��ȯ�� ȭ����ǥ�� UI_Description�� �������������� �����Ѵ�.

        Vector3 setPos = localPoint;
        int direction_settingCount = 0;

        float screenWidth = Screen.width; // ȭ���� �ʺ�
        float screenHeight = Screen.height; // ȭ���� ����
        while (true)
        {
            direction_settingCount++;
            if(direction_settingCount > 4)
            {
                _description_RectTr.pivot = new Vector2(0.5f, 0.5f);
                return OriginPos;
            }

            switch (_currnt_Dir)
            {
                case Direction.Right:
                    _description_RectTr.pivot = new Vector2(0, 1);//���� ���
                    setPos = new Vector2(localPoint.x + width / 2, localPoint.y + height / 2);
                    break;
                case Direction.Down:
                    _description_RectTr.pivot = new Vector2(0.5f, 1);//�߾� ���
                    setPos = new Vector2(localPoint.x + width / 2, localPoint.y - height / 2);
                    break;
                case Direction.Left:
                    _description_RectTr.pivot = new Vector2(1, 1);//������ ���
                    setPos = new Vector2(localPoint.x - width / 2, localPoint.y + height / 2);
                    break;
                case Direction.Up:
                    _description_RectTr.pivot = new Vector2(0.5f, 0);//�߾� �ϴ�
                    setPos = new Vector2(localPoint.x + width / 2, localPoint.y + height / 2);
                    break;
            }

            Vector3 worldPos = _description_RectTr.TransformPoint(setPos);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);//�ٽ� ��ũ����ǥ�� ��ȯ�ؼ� �����ۼ���â�� ȭ������� �������� Ȯ��

            float minPositionX = screenPos.x + (_description_RectTr.pivot.x * -1 * _description_RectTr.rect.width);// setPos.x; �״��
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