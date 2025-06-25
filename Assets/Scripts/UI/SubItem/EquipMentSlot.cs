using System;
using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using Data.Item;
using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.UIManager;
using Stats.BaseStats;
using UI.Popup.PopupUI;
using UnityEngine;
using Util;
using Zenject;

namespace UI.SubItem
{
    public class EquipMentSlot : MonoBehaviour, IItemUnEquip
    {
        public EquipmentSlotType slotType;

        private UIPlayerInventory _uiPlayerInventory;
        private Transform _contentofInventoryTr;
        private BaseStats _playerStats;
        private UIItemComponentInventory _equipedItem;
        [Inject] private IUIPopupManager _popupUIManager;
        [Inject] private ItemDataManager _itemDataManager;
        [Inject] GameManagerEx _gameManagerEx;
        public BaseStats PlayerStats
        {
            get
            {
                if(_playerStats == null)
                {
                    if(_gameManagerEx.Player != null && _gameManagerEx.Player.TryGetComponent(out BaseStats stats) == true)
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
                Managers.SceneDataSaveAndLoader.SaveEquipMentData(new KeyValuePair<EquipmentSlotType, UIItemComponentInventory>(slotType, _equipedItem));
                IsEquipped = false;//이전 아이템으로 능력치 빼기

                UIItemComponentInventory currentEquipItem = _equipedItem;
                currentEquipItem.transform.SetParent(null);
                currentEquipItem.SetItemEquipedState(false);//능력치 제거
            } 
        }

        private void ApplyItemEffects()
        {
            List<StatEffect> itemEffects = _equipedItem.ItemEffects;

            foreach (StatEffect effect in itemEffects)
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
            _uiPlayerInventory = _popupUIManager.GetImportant_Popup_UI<UIPlayerInventory>();
            _contentofInventoryTr = _uiPlayerInventory.GetComponentInChildren<InventoryContentCoordinate>().transform;

            if(Managers.SceneDataSaveAndLoader.TryGetLoadEquipMentData(slotType,out IteminfoStruct iteminfo) == true)
            {
                IItem item = _itemDataManager.GetItem(iteminfo.ItemNumber);
                UIItemComponentEquipment equipItem = item.MakeInventoryItemComponent(_popupUIManager) as UIItemComponentEquipment;
                equipItem.SetINewteminfo(iteminfo);
                equipItem.OnAfterStart += () => { equipItem.EquipItem(); };
            
            }
        }


        public void ItemEquip(UIItemComponentInventory itemComponent)
        {
            if (IsEquipped)//이미 슬롯에 아이템이 있다면
            {
                IsEquipped = false;//이전 아이템으로 능력치 빼기

                UIItemComponentInventory currentEquipItem = _equipedItem;
                currentEquipItem.transform.SetParent(_contentofInventoryTr);
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
}
