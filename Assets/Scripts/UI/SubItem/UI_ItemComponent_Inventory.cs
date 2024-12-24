using DTT.Utils.Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class UI_ItemComponent_Inventory : UI_ItemComponent
{
    public enum Images
    {
        BackGroundImage,
        ItemIconSourceImage,
        ItemGradeBorder
    }


    protected RectTransform _itemRectTr;
    protected Transform _contentofInventoryTr;
    protected UI_Player_Inventory _inventory_UI;
    protected EquipSlotTrInfo _equipSlot;
    protected Image _backGroundImage;
    protected Image _itemGradeBorder;
    protected bool _isEquipped = false;

    protected GraphicRaycaster _uiRaycaster;
    protected EventSystem _eventSystem;
    public override RectTransform ItemRectTr => _itemRectTr;
    public abstract GameObject GetLootingItemObejct(IItem iteminfo);
    protected override void AwakeInit()
    {
        Bind<Image>(typeof(Images));
        _backGroundImage = Get<Image>((int)Images.BackGroundImage);
        _itemIconSourceImage = Get<Image>((int)Images.ItemIconSourceImage);
        _itemGradeBorder = Get<Image>((int)Images.ItemGradeBorder);
        _itemRectTr = GetComponent<RectTransform>();
    }

    protected override void StartInit()
    {
        base.StartInit();
        _inventory_UI = Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>();

        _equipSlot = _inventory_UI.gameObject.FindChild<EquipSlotTrInfo>("Left_Panel", true);
        _contentofInventoryTr = _inventory_UI.GetComponentInChildren<InventoryContentCoordinate>().transform;

        _uiRaycaster = _inventory_UI.UI_Inventory_RayCaster;
        _eventSystem = _inventory_UI.EventSystem;

        _itemGradeBorder.sprite = Managers.ItemDataManager.ItemGradeBorder[_item_Grade];
    }

    public void SetItemEquipedState(bool isEquiped)
    {
        _isEquipped = isEquiped;
    }
    public sealed override void GetDragEnd(PointerEventData eventData)//�ٸ� �ڽ�Ŭ�������� GetDragEnd�� ���������� ��ӹ������ϰ� ���� ��� DropItemOnUI �޼��带 ��ӹ޾� �����ϵ���
    {//������ ��� ����
        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
        _isDragging = false;
        DropItem(eventData);
        OFFDragImage();
    }


    private void DropItem(PointerEventData eventData)
    {
        if (IsPointerOverUI(eventData, out List<RaycastResult> uiraycastResult))//UI�ʿ� ������ �ڽ�Ŭ�������� ������ ���� �޼��带 ȣ���ϰ� ����.
        {
            DropItemOnUI(eventData, uiraycastResult); // �ڽ� Ŭ������ ���� ȣ��
            return;
        }
        DropItemOnGround();
    }

    protected abstract void DropItemOnUI(PointerEventData eventData, List<RaycastResult> uiraycastResult);

    protected abstract void RemoveItemFromInventory();

    private bool IsPointerOverUI(PointerEventData eventData, out List<RaycastResult> uiraycastResult)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        uiraycastResult = results;
        return results.Count > 0;
    }

    private void DropItemOnGround()
    {
        RemoveItemFromInventory();
        GameObject lootItem = GetLootingItemObejct(_iteminfo);
        lootItem.GetComponent<LootItem>().SetDropperAndItem(_inventory_UI.InventoryOnwer, _iteminfo);
        return;
    }


    private void OFFDragImage()
    {
        DragImageIcon.gameObject.SetActive(false);
    }


    protected void AttachItemToSlot(GameObject go, Transform slot)
    {
        go.transform.SetParent(slot);
        go.GetComponent<RectTransform>().anchorMin = Vector2.zero; // ���� �ϴ� (0, 0)
        go.GetComponent<RectTransform>().anchorMax = Vector2.one;  // ���� ��� (1, 1)
        go.GetComponent<RectTransform>().offsetMin = Vector2.zero; // ������ ����
        go.GetComponent<RectTransform>().offsetMax = Vector2.zero; // ������ ����

        if (slot.GetComponent<InventoryContentCoordinate>() != null)
        {
            _isEquipped = false;
        }
        else
        {
            _isEquipped = true;
        }
    }
}
