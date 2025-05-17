using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_ItemDragImage : MonoBehaviour
{
    private UI_ItemDragImage _uI_ItemDragImage;
    void Start()
    {
        UI_ItemDragImage uI_ItemDragImage = Managers.UI_Manager.GetSceneUIFromResource<UI_ItemDragImage>();
    }
}
