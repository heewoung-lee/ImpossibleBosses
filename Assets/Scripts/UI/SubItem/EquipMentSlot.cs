using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMentSlot : MonoBehaviour,IItemUnEquip
{
    public Equipment_Slot_Type slotType;

    private UI_Player_Inventory _Player_Inventory;
    private Transform contentofInventoryTr;
    private BaseStats _playerStats;

    public Action slotEvent;


    private bool _isEquipped = false;
    public bool isEquipped {
        get => _isEquipped;
        private set
        {
            _isEquipped = value;

            if (_equipedItem == null)
                return;

            ApplyItemEffects();
        }
    }


    private UI_ItemComponent_Inventory _equipedItem;
    

    private void ApplyItemEffects()
    {
        List<StatEffect> Itemeffects = _equipedItem.ItemEffects;

        foreach (StatEffect effect in Itemeffects)
        {
            StatType statType = effect.statType;
            float statValue = effect.value;
            UpdateStatsFromEquippedItem(statType, statValue, _playerStats, isEquipped);
        }
    }

    private void UpdateStatsFromEquippedItem(StatType statType, float statValue,BaseStats stats,bool isEquipped)
    {
        int coefficient = isEquipped ? 1 : -1; //장비를 장착했으면 true, 빼면 false

        switch (statType)
        {
            case StatType.MaxHP:
                stats.Plus_MaxHp_Abillity((int)statValue * coefficient);
                break;
            case StatType.CurrentHp:
                stats.Plus_Current_Hp_Abillity((int)statValue * coefficient);
                break;
            case StatType.Attack:
                stats.Plus_Attack_Ability((int)statValue * coefficient);
                break;
            case StatType.Defence:
                stats.Plus_Defence_Abillity((int)statValue * coefficient);
                break;
            case StatType.MoveSpeed:
                stats.Plus_MoveSpeed_Abillity(statValue * coefficient);
                break;
        }
    }


    void Start()
    {
        string slotTypeName = transform.gameObject.name.Replace("_Item_Slot","");
        slotType = (Equipment_Slot_Type)Enum.Parse(typeof(Equipment_Slot_Type), slotTypeName);
        _Player_Inventory = Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>();
        contentofInventoryTr = _Player_Inventory.GetComponentInChildren<InventoryContentCoordinate>().transform;
        _playerStats = Managers.GameManagerEx.Player.GetComponent<BaseStats>();
    }

    public void ItemEquip(UI_ItemComponent_Inventory itemComponent)
    {
        if (isEquipped)//이미 슬롯에 아이템이 있다면
        {
            isEquipped = false;//이전 아이템으로 능력치 빼기

            UI_ItemComponent_Inventory currentEquipItem = _equipedItem;
            currentEquipItem.transform.SetParent(contentofInventoryTr);
            currentEquipItem.SetItemEquipedState(false);

            _equipedItem = itemComponent;
            isEquipped = true;//새로낀 아이템으로 능력치를 적용
        }
        else
        {
            _equipedItem = itemComponent;
            isEquipped = true;
        }
    }

    public void ItemUnEquip()
    {
        isEquipped = false;
    }

}
