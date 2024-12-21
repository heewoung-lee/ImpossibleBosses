using System;
using UnityEngine;

public interface IInventoryItemMaker
{
    public UI_ItemComponent_Inventory MakeItemComponent(Transform parent = null, int itemCount = 1, string name = null, string path = null);
}