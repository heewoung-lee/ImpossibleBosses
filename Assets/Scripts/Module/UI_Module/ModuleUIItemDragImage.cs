using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleUIItemDragImage : MonoBehaviour
    {
        private UI_ItemDragImage _uiItemDragImage;
        void Start()
        {
            UI_ItemDragImage uIItemDragImage = Managers.UIManager.GetSceneUIFromResource<UI_ItemDragImage>();
        }
    }
}
