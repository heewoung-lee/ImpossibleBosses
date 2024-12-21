using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDragImage : UI_Scene
{
    private Image _itemDragImage;
    public Image ItemDragImage => _itemDragImage;
    void Start()
    {
        _itemDragImage = Utill.FindChild<Image>(gameObject, "ItemDragImage");
        GetComponent<Canvas>().sortingOrder = 50;
        _itemDragImage.gameObject.SetActive(false);
    }

}
