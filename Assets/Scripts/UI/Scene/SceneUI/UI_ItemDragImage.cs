using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDragImage : UI_Scene
{
    private Image _itemDragImage;

    enum Images
    {
        ItemDragImage
    }


    public bool IsDragImageActive
    {
        get
        {
            return _itemDragImage.IsActive();
        }
    }
    void Awake()
    {
        Bind<Image>(typeof(Images));

        _itemDragImage = Get<Image>((int)Images.ItemDragImage);
        SetSortingOrder((int)Define.SpecialSortingOrder.DragImage);
        _itemDragImage.gameObject.SetActive(false);
    }
    public void SetImageSprite(Sprite sprite)
    {
        _itemDragImage.sprite = sprite;
    }
    public void SetDragImagePosition(Vector3 pos)
    {
        _itemDragImage.transform.position = pos;
    }
    public void SetImageSpriteColorAlpah(float alphaValue)
    {
        _itemDragImage.color = new Color(
            _itemDragImage.color.r,
            _itemDragImage.color.g,
            _itemDragImage.color.b,
            alphaValue);
    }
    public void SetItemImageEnable()
    {
        _itemDragImage.gameObject.SetActive(true);
    }
    public void SetItemImageDisable()
    {
        _itemDragImage.gameObject.SetActive(false);
    }

}
