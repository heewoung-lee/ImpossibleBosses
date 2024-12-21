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
        //�������� �ƴ϶�� ���Կ� �ְ�, �ɷ�ġ ����
        //�������̶�� ������â�� ��������, �ɷ�ġ ����
        if (_isEquipped == false) // �������� �������� �ƴ϶�� �����ϴ� ���� ����
        {
            ItemEquipment equip = _iteminfo as ItemEquipment;
            Equipment_Slot_Type eqiupSlot = equip.Equipment_Slot;
            EquipItemToSlot(eqiupSlot);
        }
        else// �������̶�� ��������
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
        //�巡�� �� UI���� ��ó�� ����Ѵٸ� ���Կ� ��������
        //�巡�� �� UI�ۿ� ����� �Ѵٸ� �������� ����������
        //�巡�� �� �׿��������� ����Ѵٸ� �ٽ� ������â���ΰ���

        // 1. UI ����ĳ��Ʈ �õ�
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
