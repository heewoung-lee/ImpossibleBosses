using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_ItemComponent_Equipment : UI_ItemComponent_Inventory
{

    protected override void AwakeInit()
    {
        base.AwakeInit();
    }

    protected override void StartInit()
    {
        base.StartInit();
    }

    public override void ItemRightClick(PointerEventData eventdata)
    {
        //장착중이 아니라면 슬롯에 넣고, 능력치 적용
        //장착중이라면 아이템창에 돌려놓고, 능력치 감소
        if (_isEquipped == false) // 아이템이 장착중이 아니라면 장착하는 로직 수행
        {
            ItemEquipment equip = _iteminfo as ItemEquipment;
            Equipment_Slot_Type eqiupSlot = equip.Equipment_Slot;
            EquipItemToSlot(eqiupSlot);
        }
        else// 장착중이라면 장착해제
        {
            GetComponentInParent<EquipMentSlot>().ItemUnEquip();
            AttachItemToSlot(gameObject, _contentofInventoryTr);
        }
    }

    public void EquipItemToSlot(Equipment_Slot_Type eqiupSlot)
    {
        EquipMentSlot slot = null;

        switch (eqiupSlot)
        {
            case Equipment_Slot_Type.Helmet:
                slot = _equipSlot.HelmetEquipMent;
                break;
            case Equipment_Slot_Type.Gauntlet:
                slot = _equipSlot.GauntletEquipMent;
                break;
            case Equipment_Slot_Type.Shoes:
                slot = _equipSlot.ShoesEquipMent;
                break;
            case Equipment_Slot_Type.Weapon:
                slot = _equipSlot.WeaponEquipMent;
                break;
            case Equipment_Slot_Type.Ring:
                slot = _equipSlot.RingEquipMent;
                break;
            case Equipment_Slot_Type.Armor:
                slot = _equipSlot.ArmorEquipMent;
                break;
        }
        slot.ItemEquip(this);
        AttachItemToSlot(gameObject, slot.transform);
    }

    public override void GetDragEnd(PointerEventData eventData)
    {
        _itemIconSourceImage.color = new Color(_itemIconSourceImage.color.r, _itemIconSourceImage.color.g, _itemIconSourceImage.color.b, 1f);
        _isDragging = false;
        //드래그 시 UI슬롯 근처로 드랍한다면 슬롯에 끼워지기
        //드래그 시 UI밖에 드랍을 한다면 아이템이 떨어지도록
        //드래그 시 그외지역에서 드랍한다면 다시 아이템창으로가기

        // 1. UI 레이캐스트 시도
        List<RaycastResult> uiResults = new List<RaycastResult>();
        _uiRaycaster.Raycast(eventData, uiResults);

        foreach (RaycastResult uiResult in uiResults)
        {
            if (uiResult.gameObject.tag == "EquipSlot" && _iteminfo is ItemEquipment)
            {
                EquipMentSlot slot = uiResult.gameObject.GetComponent<EquipMentSlot>();
                ItemEquipment equipment = _iteminfo as ItemEquipment;

                if (slot.slotType == equipment.Equipment_Slot)
                {
                    EquipItemToSlot(equipment.Equipment_Slot);
                }
            }
            else if(uiResult.gameObject.TryGetComponentInChildren(out InventoryContentCoordinate contentTr) && _isEquipped == true)
            {
                GetComponentInParent<EquipMentSlot>().ItemUnEquip();
                AttachItemToSlot(gameObject, contentTr.transform);
            }
        }
        DragImageIcon.gameObject.SetActive(false);
    }

   
}
