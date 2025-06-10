using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryItemSaveAndLoader : MonoBehaviour
{
    //아이템 정보 가져오기
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
            //씬 전환후 가져온 아이템들에 대한 후처리는 여기에 할것 
        }
    }

}
