using GameManagers;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIPlayerTestButton : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;
        void Start()
        {
            UICreateItemAndGoldButton buttonUI = _uiManager.GetSceneUIFromResource<UICreateItemAndGoldButton>();
        }

    }
}
