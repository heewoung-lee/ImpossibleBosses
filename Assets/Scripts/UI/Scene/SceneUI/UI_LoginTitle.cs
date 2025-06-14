using GameManagers;
using UI.Popup.PopupUI;
using UnityEngine.UI;

namespace UI.Scene.SceneUI
{
    public class UI_LoginTitle : UIScene
    {
        enum ButtonEvent
        {
            Button_Start,
        }


        Button _openLoginButton;
        UILoginPopup _uiLoginPopup;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Button>(typeof(ButtonEvent));
            _openLoginButton = Get<Button>((int)ButtonEvent.Button_Start);
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
                _uiLoginPopup = Managers.UIManager.GetPopupUIFromResource<UILoginPopup>();
            }
            Managers.UIManager.ShowPopupUI(_uiLoginPopup);
        }


    }
}
