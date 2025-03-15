using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemComponent_Consumable : UI_ItemComponent_Inventory
{
    enum Texts
    {
        ItemCountText
    }

    private TMP_Text _itemCount_Text;
    private string _itemGUID;
    private UI_ConsumableBar _consumableBar;
    private int _itemCount;
    private float _duringbuff;

    public float DuringBuffTime => _duringbuff;
    public TMP_Text ItemCount_Text => _itemCount_Text;
    public string ItemGUID => _itemGUID;
    public int ItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            _itemCount_Text.text = _itemCount.ToString();
        }
    }


    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_Text>(typeof(Texts));
        _itemCount_Text = Get<TMP_Text>((int)Texts.ItemCountText);
        _itemGUID = System.Guid.NewGuid().ToString();
    }

    protected override void StartInit()
    {
        base.StartInit();
        _consumableBar = Managers.UI_Manager.Get_Scene_UI<UI_ConsumableBar>();
        _itemCount_Text.text = $"{_itemCount}";
        CombineConsumableItems();
    }

    public bool CombineConsumableItems(Transform parentTr = null)//�Һ� ������ ������� ���� �Һ�������� �߰� �Ҷ� �̹� �ִ� ������ �ִ� ��� �߰��ϱ�.
    {
        Transform searchingTr = parentTr;
        if (parentTr == null)
        {
            searchingTr = gameObject.transform.parent;
        }

        foreach (Transform item_In_Inventory in searchingTr)
        {
            if (item_In_Inventory.TryGetComponent(out UI_ItemComponent_Consumable item))
            {
                if (item.ItemGUID == _itemGUID)
                    continue;

                if (item.ItemNumber == ItemNumber)
                {
                    item.ItemCount += int.Parse(_itemCount_Text.text);
                    Managers.ResourceManager.DestroyObject(gameObject);
                    return true;
                }
            }
        }
        return false;
    }
    public override void ItemRightClick(PointerEventData eventdata)
    {
        base.ItemRightClick(eventdata);
        //�������� �ƴ϶�� ���Կ� �ְ�
        //�������̶�� ������â�� ��������
        if (_isEquipped == false) // �������� �������� �ƴ϶�� �����ϴ� ���� ����
        {
            ItemConsumable consumable = _iteminfo as ItemConsumable;
            ConsumableItemEquip(this);

        }
        else// �������̶�� ��������
        {
            gameObject.transform.SetParent(_contentofInventoryTr);
            _isEquipped = false;
            CombineConsumableItems();
        }

    }
    public void ConsumableItemEquip(UI_ItemComponent_Consumable itemcomponent)
    {
        bool isCheckCombine = false;
        CloseDescription();//���ִ� ������ ����â �ݱ�

        foreach (Transform parentTr in _consumableBar.FrameTrs)
        {
            if (isCheckCombine = CombineConsumableItems(parentTr))
                return;
        }
        //�Һ�â�� ���� ������ �ִ��� Ȯ���Ѵ�.
        //���� ������ ���� ���, ������ ���ϰ� �ϳ��� ��ģ��.
        //���� �ѷ����Ҵµ���, ���� ������ ���ٸ�, ����ִ� �Һ�â�� Ȯ��
        //����ִ� �Һ�â�� ������ �ִ´�.
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
        //�巡�� �� UI���� ��ó�� ����Ѵٸ� ���Կ� ��������
        //�巡�� �� UI�ۿ� ����� �Ѵٸ� �������� ����������
        //�巡�� �� �׿��������� ����Ѵٸ� �ٽ� ������â���ΰ���

        foreach (RaycastResult uiResult in uiraycastResult)
        {
            if (uiResult.gameObject.tag == "ConsumableSlot" && _iteminfo is ItemConsumable)
            {
                foreach (Transform frameTr in _consumableBar.FrameTrs)//���� ������ �������� �ִ��� ���� üũ
                {
                    if (frameTr.gameObject.TryGetComponentInChildren(out UI_ItemComponent_Consumable ui_consumableItem))
                    {
                        if (ui_consumableItem.ItemNumber != _iteminfo.ItemNumber)
                            continue;

                        CombineConsumableItems(frameTr);//���ٸ� ������ ä���ش�.
                        break;
                    }
                }

                if (uiResult.gameObject.TryGetComponentInChildren(out UI_ItemComponent_Consumable ui_alreadyitem)//���Գ����� ������ �Ѵٸ�
                    && ui_alreadyitem.ItemNumber != _iteminfo.ItemNumber)
                {
                    AttachItemToSlot(ui_alreadyitem.gameObject, transform.parent);
                }

                AttachItemToSlot(gameObject, uiResult.gameObject.transform);//ĭ�� ����ִٸ� ����
                break;
            }

            else if (uiResult.gameObject.TryGetComponentInChildren(out InventoryContentCoordinate contextTr))
            {
                AttachItemToSlot(gameObject, contextTr.transform);
            }
        }
    }

    public void IntializeItem(IItem iteminfo, int count)
    {
        base.IntializeItem(iteminfo);
        _itemCount += count;
        _duringbuff = (iteminfo as ItemConsumable).duration;
    }

    public override GameObject GetLootingItemObejct(IItem iteminfo)
    {
        GameObject lootitem = Managers.ResourceManager.InstantiatePrefab("LootingItem/Potion", Managers.LootItemManager.ItemRoot);
        lootitem.GetComponent<LootItem>().SetIteminfo(iteminfo);
        return lootitem;
    }


    protected override void RemoveItemFromInventory()
    {
        ItemCount--;
        if (ItemCount <= 0)
            Managers.ResourceManager.DestroyObject(gameObject);
    }
}
