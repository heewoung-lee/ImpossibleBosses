using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using Data.Item;
using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using NetWork.LootItem;
using TMPro;
using UI.Scene.SceneUI;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;
using Zenject;

namespace UI.SubItem
{
    public class UIItemComponentConsumable : UIItemComponentInventory
    {
        enum Texts
        {
            ItemCountText
        }

        private TMP_Text _itemCountText;
        private string _itemGuid;
        private UIConsumableBar _consumableBar;
        private int _itemCount;
        private float _duringbuff;
        [Inject] private IUISceneManager _uiManager;
        [Inject] private IInstantiate _instantiate;
        [Inject] private IDestroyObject _destroyer;

        public float DuringBuffTime => _duringbuff;
        public string ItemGuid => _itemGuid;
        public int ItemCount
        {
            get => _itemCount;
            set
            {
                _itemCount = value;
                _itemCountText.text = _itemCount.ToString();
            }
        }


        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<TMP_Text>(typeof(Texts));
            _itemCountText = Get<TMP_Text>((int)Texts.ItemCountText);
            _itemGuid = System.Guid.NewGuid().ToString();
        }

        protected override void StartInit()
        {
            base.StartInit();
            _consumableBar = _uiManager.Get_Scene_UI<UIConsumableBar>();
            _itemCountText.text = $"{_itemCount}";
            CombineConsumableItems();
        }

        public bool CombineConsumableItems(Transform parentTr = null)//소비 아이템 종류라면 같은 소비아이템을 추가 할때 이미 있는 물약이 있는 경우 추가하기.
        {
            Transform searchingTr = parentTr;
            if (parentTr == null)
            {
                searchingTr = gameObject.transform.parent;
            }

            foreach (Transform itemInInventory in searchingTr)
            {
                if (itemInInventory.TryGetComponent(out UIItemComponentConsumable item))
                {
                    if (item.ItemGuid == _itemGuid)
                        continue;

                    if (item.ItemNumber == ItemNumber)
                    {
                        item.ItemCount += int.Parse(_itemCountText.text);
                        _destroyer.DestroyObject(gameObject);
                        return true;
                    }
                }
            }
            return false;
        }
        public override void ItemRightClick(PointerEventData eventdata)
        {
            base.ItemRightClick(eventdata);
            //장착중이 아니라면 슬롯에 넣고
            //장착중이라면 아이템창에 돌려놓고
            if (IsEquipped == false) // 아이템이 장착중이 아니라면 장착하는 로직 수행
            {
                ItemConsumable consumable = _iteminfo as ItemConsumable;
                ConsumableItemEquip(this);

            }
            else// 장착중이라면 장착해제
            {
                gameObject.transform.SetParent(_contentofInventoryTr);
                SetItemEquipedState(false);
                CombineConsumableItems();
            }

        }
        public void ConsumableItemEquip(UIItemComponentConsumable itemcomponent)
        {
            CloseDescription();//떠있는 아이템 설명창 닫기

            foreach (Transform parentTr in _consumableBar.FrameTrs)
            {
                if (CombineConsumableItems(parentTr))
                    return;
            }
            //소비창에 같은 물약이 있는지 확인한다.
            //같은 물약이 있을 경우, 갯수를 더하고 하나로 합친다.
            //전부 둘러보았는데도, 같은 물약이 없다면, 비어있는 소비창을 확인
            //비어있는 소비창에 물약을 넣는다.
            for (int i = 0; i < _consumableBar.FrameTrs.Length; i++)
            {
                if (_consumableBar.FrameTrs[i].childCount < 1)
                {
                    AttachItemToSlot(itemcomponent.gameObject, _consumableBar.FrameTrs[i].transform);
                    break;
                }
            }

        }

        protected override void DropItemOnUI(PointerEventData eventData, List<RaycastResult> uiraycastResult)
        {
            //드래그 시 UI슬롯 근처로 드랍한다면 슬롯에 끼워지기
            //드래그 시 UI밖에 드랍을 한다면 아이템이 떨어지도록
            //드래그 시 그외지역에서 드랍한다면 다시 아이템창으로가기

            foreach (RaycastResult uiResult in uiraycastResult)
            {
                if (uiResult.gameObject.CompareTag("ConsumableSlot") && _iteminfo is ItemConsumable)
                {
                    foreach (Transform frameTr in _consumableBar.FrameTrs)//같은 종류의 아이템이 있는지 먼저 체크
                    {
                        if (frameTr.gameObject.TryGetComponentInChildren(out UIItemComponentConsumable uiConsumableItem))
                        {
                            if (uiConsumableItem.ItemNumber != _iteminfo.ItemNumber)
                                continue;

                            CombineConsumableItems(frameTr);//같다면 수량을 채워준다.
                            break;
                        }
                    }

                    if (uiResult.gameObject.TryGetComponentInChildren(out UIItemComponentConsumable uiAlreadyitem)//슬롯끼리의 스왑을 한다면
                        && uiAlreadyitem.ItemNumber != _iteminfo.ItemNumber)
                    {
                        AttachItemToSlot(uiAlreadyitem.gameObject, transform.parent);
                    }

                    AttachItemToSlot(gameObject, uiResult.gameObject.transform);//칸이 비어있다면 설정
                    break;
                }

                else if (uiResult.gameObject.TryGetComponentInChildren(out InventoryContentCoordinate contextTr))
                {
                    AttachItemToSlot(gameObject, contextTr.transform);
                }
            }
        }

        public void InitializeItem(IItem iteminfo, int count)
        {
            base.IntializeItem(iteminfo);
            _itemCount += count;
            _duringbuff = ((ItemConsumable)iteminfo).duration;
        }

        public override GameObject GetLootingItemObejct(IItem iteminfo)
        {
            GameObject lootitem = _instantiate.InstantiateByPath("Prefabs/LootingItem/Potion", Managers.LootItemManager.ItemRoot);
            lootitem.GetComponent<LootItem>().SetIteminfo(iteminfo);
            return lootitem;
        }


        protected override void RemoveItemFromInventory()
        {
            ItemCount--;
            if (ItemCount <= 0)
                _destroyer.DestroyObject(gameObject);
        }
    }
}
