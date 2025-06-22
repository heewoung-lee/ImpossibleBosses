using GameManagers;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIConsumableBar : MonoBehaviour
    {
        [Inject]private UIManager _uiManager; 
        void Start()
        {
            UIConsumableBar uiConsumableBar = _uiManager.GetSceneUIFromResource<UIConsumableBar>();
        }

    }
}
