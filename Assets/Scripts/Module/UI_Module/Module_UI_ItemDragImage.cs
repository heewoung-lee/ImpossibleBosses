using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_UI_ItemDragImage : MonoBehaviour
{
    private UI_ItemDragImage _uI_ItemDragImage;
    void Start()
    {
        UI_ItemDragImage uI_ItemDragImage = Managers.UIManager.GetSceneUIFromResource<UI_ItemDragImage>();
    }
}
