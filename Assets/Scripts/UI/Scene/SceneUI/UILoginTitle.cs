using GameManagers;
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
        [Inject] private UIManager _uiManager;

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
                _uiLoginPopup = _uiManager.GetPopupUIFromResource<UILoginPopup>();
            }
            _uiManager.ShowPopupUI(_uiLoginPopup);
        }


    }
}
