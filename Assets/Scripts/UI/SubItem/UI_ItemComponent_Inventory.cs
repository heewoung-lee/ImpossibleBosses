using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_ItemComponent_Inventory : UI_ItemComponent
{
    public enum Images
    {
        BackGroundImage,
        ItemIconSourceImage,
        ItemGradeBorder
    }
    private bool _isEquipped = false;
    private Action _onAfterStart;

    protected RectTransform _itemRectTr;
    protected Transform _contentofInventoryTr;
    protected UI_Player_Inventory _inventory_UI;
    protected EquipSlotTrInfo _equipSlot;
    protected Image _backGroundImage;
    protected Image _itemGradeBorder;
    protected GraphicRaycaster _uiRaycaster;
    protected EventSystem _eventSystem;

    public event Action OnAfterStart
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _onAfterStart, value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _onAfterStart, value);
        }
    }
    public bool IsEquipped => _isEquipped;
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


        _onAfterStart?.Invoke();
        _onAfterStart = null;

    }

    public void SetItemEquipedState(bool isEquiped)
    {
        _isEquipped = isEquiped;
    }
    public sealed override void GetDragEnd(PointerEventData eventData)//다른 자식클래스들이 GetDragEnd를 직접적으로 상속받지못하게 막고 대신 DropItemOnUI 메서드를 상속받아 구현하도록
    {//아이템 드랍 구현
        if (_isDragging)
        {
            DropItem(eventData);
            RevertImage();
        }
    }


    private void DropItem(PointerEventData eventData)
    {
        if (IsPointerOverUI(eventData, out List<RaycastResult> uiraycastResult))//UI쪽에 닿으면 자식클래스에서 구현해 놓은 메서드를 호출하고 종료.
        {
            DropItemOnUI(eventData, uiraycastResult); // 자식 클래스의 구현 호출
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

    protected virtual void DropItemOnGround()
    {
        RemoveItemFromInventory();
        IteminfoStruct itemStruct = new IteminfoStruct(_iteminfo);
        Managers.RelayManager.NGO_RPC_Caller.Spawn_Loot_ItemRpc(itemStruct,Managers.GameManagerEx.Player.transform.position);
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
