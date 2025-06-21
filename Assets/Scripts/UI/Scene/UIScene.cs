using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.UI_Interface;
using UI.Popup.PopupUI;
using UnityEngine;
using Zenject;

namespace UI.Scene
{
    public class UIScene : UIBase
    {
        [Inject] private IUIManager _uiManager;
        protected override void AwakeInit()
        {
            _uiManager.SetCanvas(gameObject.GetComponent<Canvas>(), true);
        }
        protected override void StartInit()
        {
        }
    }
}
