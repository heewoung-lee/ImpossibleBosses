using GameManagers;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIPlayerInfo : MonoBehaviour
    {
        [Inject] private UIManager _uimanager;
        void Start()
        {
            UIPlayerInfo playerInfoUI = _uimanager.GetSceneUIFromResource<UIPlayerInfo>();
        }
    }
}
