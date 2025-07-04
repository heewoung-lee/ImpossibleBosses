using System.Collections.Generic;
using Unity.Netcode;
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
                _itemRoot = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/ItemRootNetwork");
            }
            return _itemRoot.transform;
        }
    }
}