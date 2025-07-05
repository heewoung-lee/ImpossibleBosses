using GameManagers;
using GameManagers.Interface.UIManager;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIConsumableBar : MonoBehaviour
    {
        [Inject]private IUISceneManager _uiSceneManager; 
        void Start()
        {
            UIConsumableBar uiConsumableBar = _uiSceneManager.GetSceneUIFromResource<UIConsumableBar>();
        }

    }
}
