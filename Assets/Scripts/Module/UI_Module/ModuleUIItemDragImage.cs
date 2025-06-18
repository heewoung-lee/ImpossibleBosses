using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIItemDragImage : MonoBehaviour
    {
        private UI_ItemDragImage _uiItemDragImage;
        
        [Inject]private UIManager _uiManager; 
        void Start()
        {
            UI_ItemDragImage uIItemDragImage = _uiManager.GetSceneUIFromResource<UI_ItemDragImage>();
        }
    }
}
