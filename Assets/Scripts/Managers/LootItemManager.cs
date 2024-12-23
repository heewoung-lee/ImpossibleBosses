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
}