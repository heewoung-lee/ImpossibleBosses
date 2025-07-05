using GameManagers;
using GameManagers.Interface.UIManager;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIBufferBar : MonoBehaviour
    {
        [Inject]private IUISceneManager _uiSceneManager; 
        UIBufferBar _uiBufferbar;

        void Start()
        {
            _uiBufferbar = _uiSceneManager.GetSceneUIFromResource<UIBufferBar>();
        }
    }
}
