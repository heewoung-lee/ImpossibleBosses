using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
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

    protected override void DropItemOnUI(PointerEventData eventData, List<RaycastResult> uiraycastResult)
    {
        foreach (RaycastResult uiResult in uiraycastResult)
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
    }

    public override GameObject GetLootingItemObejct(IItem iteminfo)
    {
        switch ((iteminfo as ItemEquipment).Equipment_Slot)
        {
            case Equipment_Slot_Type.Helmet:
            case Equipment_Slot_Type.Armor:
                return Managers.ResourceManager.InstantiatePrefab("LootingItem/Shield", Managers.LootItemManager.ItemRoot);
            case Equipment_Slot_Type.Weapon:
                return Managers.ResourceManager.InstantiatePrefab("LootingItem/Sword", Managers.LootItemManager.ItemRoot);
            default:
                return Managers.ResourceManager.InstantiatePrefab("LootingItem/Bag", Managers.LootItemManager.ItemRoot);
        }
    }

    protected override void RemoveItemFromInventory()
    {
        Managers.ResourceManager.DestroyObject(gameObject);
    }
}
