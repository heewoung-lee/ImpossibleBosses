using GameManagers;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIBufferBar : MonoBehaviour
    {
        [Inject]private UIManager _uiManager; 
        UIBufferBar _uiBufferbar;

        void Start()
        {
            _uiBufferbar = _uiManager.GetSceneUIFromResource<UIBufferBar>();
        }
    }
}
