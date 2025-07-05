using GameManagers;
using GameManagers.Interface.UIManager;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIItemDragImage : MonoBehaviour
    {
        private UI_ItemDragImage _uiItemDragImage;
        
        [Inject]private IUISceneManager _uisceneManager; 
        void Start()
        {
            UI_ItemDragImage uIItemDragImage = _uisceneManager.GetSceneUIFromResource<UI_ItemDragImage>();
        }
    }
}
