using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using System;
using System.Collections;
using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using Data.Item;
using GameManagers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipMentSlot : MonoBehaviour, IItemUnEquip
{
    public EquipmentSlotType slotType;

    private UI_Player_Inventory _player_Inventory;
    private Transform contentofInventoryTr;
    private BaseStats _playerStats;
    private UI_ItemComponent_Inventory _equipedItem;

    public Action slotEvent;


    public BaseStats PlayerStats
    {
        get
        {
            if(_playerStats == null)
            {
                if(Managers.GameManagerEx.Player != null && Managers.GameManagerEx.Player.TryGetComponent(out BaseStats stats) == true)
                {
                    _playerStats = stats;
                }
            }
            return _playerStats;
        }
    }


    private bool _isEquipped = false;
    public bool IsEquipped
    {
        get => _isEquipped;
        private set
        {
            _isEquipped = value;

            if (_equipedItem == null)
                return;

            ApplyItemEffects();
        }
    }



    private void OnDestroy()
    {
        //TODO: 왜 디스트로이에 했냐, OnDisable로 하면 화면이 닫힐때 호출이 안되므로 디스트로이에 저장 로직 만듬

        SaveDataFromEquipment();
    }
    private void SaveDataFromEquipment()
    {
        if (IsEquipped == true)
        {
            Managers.SceneDataSaveAndLoader.SaveEquipMentData(new KeyValuePair<EquipmentSlotType, UI_ItemComponent_Inventory>(slotType, _equipedItem));
            IsEquipped = false;//이전 아이템으로 능력치 빼기

            UI_ItemComponent_Inventory currentEquipItem = _equipedItem;
            currentEquipItem.transform.SetParent(null);
            currentEquipItem.SetItemEquipedState(false);//능력치 제거
        } 
    }

    private void ApplyItemEffects()
    {
        List<StatEffect> Itemeffects = _equipedItem.ItemEffects;

        foreach (StatEffect effect in Itemeffects)
        {
            StatType statType = effect.statType;
            float statValue = effect.value;
            UpdateStatsFromEquippedItem(statType, statValue, PlayerStats, IsEquipped);
        }
    }

    private void UpdateStatsFromEquippedItem(StatType statType, float statValue, BaseStats stats, bool isEquipped)
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
        string slotTypeName = transform.gameObject.name.Replace("_Item_Slot", "");
        slotType = (EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), slotTypeName);
        _player_Inventory = Managers.UIManager.GetImportant_Popup_UI<UI_Player_Inventory>();
        contentofInventoryTr = _player_Inventory.GetComponentInChildren<InventoryContentCoordinate>().transform;

        if(Managers.SceneDataSaveAndLoader.TryGetLoadEquipMentData(slotType,out IteminfoStruct iteminfo) == true)
        {
            IItem item = Managers.ItemDataManager.GetItem(iteminfo.ItemNumber);
            UI_ItemComponent_Equipment equipItem = item.MakeInventoryItemComponent() as UI_ItemComponent_Equipment;
            equipItem.SetINewteminfo(iteminfo);
            equipItem.OnAfterStart += () => { equipItem.EquipItem(); };
            
        }
    }


    public void ItemEquip(UI_ItemComponent_Inventory itemComponent)
    {
        if (IsEquipped)//이미 슬롯에 아이템이 있다면
        {
            IsEquipped = false;//이전 아이템으로 능력치 빼기

            UI_ItemComponent_Inventory currentEquipItem = _equipedItem;
            currentEquipItem.transform.SetParent(contentofInventoryTr);
            currentEquipItem.SetItemEquipedState(false);
        }
        _equipedItem = itemComponent;
        IsEquipped = true;
    }

    public void ItemUnEquip()
    {
        IsEquipped = false;
    }

}
