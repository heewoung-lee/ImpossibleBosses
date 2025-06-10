using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryItemSaveAndLoader : MonoBehaviour
{
    //������ ���� ��������
    List<IteminfoStruct> _inventoryItemList = new List<IteminfoStruct>();

    private void OnDestroy()
    {
        foreach (UI_ItemComponent_Inventory item in GetComponentsInChildren<UI_ItemComponent_Inventory>())
        {
            _inventoryItemList.Add(new IteminfoStruct(item));
        }
        Managers.SceneDataSaveAndLoader.SaveInventoryItem(_inventoryItemList);
    }

    private void Start()
    {
        if(Managers.SceneDataSaveAndLoader.TryGetLoadInventoryItem(out List<UI_ItemComponent_Inventory> loaditemList))
        {
            //�� ��ȯ�� ������ �����۵鿡 ���� ��ó���� ���⿡ �Ұ� 
        }
    }

}
