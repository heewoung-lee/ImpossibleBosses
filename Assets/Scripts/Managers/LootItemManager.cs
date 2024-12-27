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


    private GameObject _lootingItemRoot;

    public Transform LootingItemRoot
    {
        get
        {
            if(_lootingItemRoot == null)
            {
                _lootingItemRoot = new GameObject("@LootItemRoot");
            }
            return _lootingItemRoot.transform;  
        }
    }
}