using GameManagers;
using GameManagers.Interface;
using UI.Popup.PopupUI;
using UnityEngine.UI;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UILoginTitle : UIScene
    {
        enum ButtonEvent
        {
            ButtonStart,
        }

        private Button _openLoginButton;
        private UILoginPopup _uiLoginPopup;
        [Inject] private IUIPopupManager _popUpManager;

        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Button>(typeof(ButtonEvent));
            _openLoginButton = Get<Button>((int)ButtonEvent.ButtonStart);
        }

        protected override void StartInit()
        {
            base.StartInit();
            _openLoginButton.onClick.AddListener(ClickLoginButton);
        }
    

        public void ClickLoginButton()
        {
            if(_uiLoginPopup == null)
            {
                _uiLoginPopup = _popUpManager.GetPopupUIFromResource<UILoginPopup>();
            }
            _popUpManager.ShowPopupUI(_uiLoginPopup);
        }


    }
}
