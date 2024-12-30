using System.Collections.Generic;
using UnityEngine;

public class LootItemManager
{
    private GameObject _itemRoot;

    public Transform ItemRoot
    {
        get
        {
            if(_itemRoot == null)
            {
                _itemRoot = new GameObject("@ItemRoot");
            }
            return _itemRoot.transform;
        }
    }


    private GameObject _temporaryInventory;

    public Transform TemporaryInventory
    {
        get
        {
            if(_temporaryInventory == null)
            {
                _temporaryInventory = new GameObject("@LootItemRoot");
            }
            return _temporaryInventory.transform;  
        }
    }
}