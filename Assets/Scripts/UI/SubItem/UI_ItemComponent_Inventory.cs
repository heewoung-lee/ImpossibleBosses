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
    public override void GetDragEnd(PointerEventData eventData)
    {//아이템 드랍 구현
        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
        _isDragging = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 위치로부터 광선 생성
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) // 바닥 오브젝트에 Tag 설정 필요
            {
                Debug.Log($"Dropped on ground at: {hit.point}");

                //TODO:아이템 떨어뜨리기
                GameObject lootItem =  Managers.ResourceManager.InstantiatePrefab("LootingItem/Sword",Managers.LootItemManager.ItemRoot);
                lootItem.GetComponent<LootItem>().SetDropperAndItem(_inventory_UI.InventoryOnwer,_iteminfo);
                DragImageIcon.gameObject.SetActive(false);
                return;
            }
        }

    }
    protected void AttachItemToSlot(GameObject go, Transform slot)
    {
        go.transform.SetParent(slot);
        go.GetComponent<RectTransform>().anchorMin = Vector2.zero; // 좌측 하단 (0, 0)
        go.GetComponent<RectTransform>().anchorMax = Vector2.one;  // 우측 상단 (1, 1)
        go.GetComponent<RectTransform>().offsetMin = Vector2.zero; // 오프셋 제거
        go.GetComponent<RectTransform>().offsetMax = Vector2.zero; // 오프셋 제거

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
