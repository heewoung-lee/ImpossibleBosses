using GameManagers;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Popup.PopupUI
{
    public class UISignUpPopup : IDPwPopup, IUIHasCloseButton
    {
        Button _buttonClose;
        Button _buttonSignup;
        TMP_InputField _idInputField;
        TMP_InputField _pwInputField;
        private UIAlertPopupBase _alertPopup;
        private UIAlertPopupBase _confirmPopup;

        public override TMP_InputField IdInputField => _idInputField;

        public override TMP_InputField PwInputField => _pwInputField;

        public Button CloseButton => _buttonClose;

        enum Buttons
        {
            ButtonClose,
            ButtonSignup
        }
        enum InputFields
        {
            IDInputField,
            PwInputField
        }

        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<Button>(typeof(Buttons));
            _idInputField = Get<TMP_InputField>((int)InputFields.IDInputField);
            _pwInputField = Get<TMP_InputField>((int)InputFields.PwInputField);
            _buttonClose = Get<Button>((int)Buttons.ButtonClose);
            _buttonSignup = Get<Button>((int)Buttons.ButtonSignup);
            _buttonClose.onClick.AddListener(OnClickCloseButton);
            _buttonSignup.onClick.AddListener(CreateID);
        }
        public async void CreateID()
        {
            if (string.IsNullOrEmpty(_idInputField.text) || string.IsNullOrEmpty(_pwInputField.text))
                return;


            (bool isCheckResult, string message) =  await Managers.LogInManager.WriteToGoogleSheet(_idInputField.text,_pwInputField.text);

            if(isCheckResult == false)
            {
                _alertPopup = ShowAlertDialogUI<UIAlertDialog>(_alertPopup, "오류", message);
            }
            else
            {
                _confirmPopup = ShowAlertDialogUI<UIConfirmDialog>(_confirmPopup, "성공", message, ShowLoginAfterSignUp);
                ClearIDAndPw();
            }
        }

        public void ClearIDAndPw()
        {
            _idInputField.text = "";
            _pwInputField.text = "";
        }

        public void ShowLoginAfterSignUp()
        {
            Managers.UIManager.CloseAllPopupUI();
            UILoginPopup uiLoginPopup = Managers.UIManager.GetImportant_Popup_UI<UILoginPopup>();
            Managers.UIManager.ShowPopupUI(uiLoginPopup);
        }

        private UIAlertPopupBase ShowAlertDialogUI<T>(UIAlertPopupBase alertBasePopup,string titleMessage,string bodyText,UnityAction closeButtonAction = null) where T: UIAlertPopupBase
        {
            if(alertBasePopup == null)
            {
                alertBasePopup = Managers.UIManager.GetPopupInDict<T>();
            }
            alertBasePopup.SetText(titleMessage, bodyText);
            if (closeButtonAction != null)
            {
                alertBasePopup.SetCloseButtonOverride(closeButtonAction);
            }
            Managers.UIManager.ShowPopupUI(alertBasePopup);

            return alertBasePopup;
        }

        protected override void StartInit()
        {
        }

        public void OnClickCloseButton()
        {
            Managers.UIManager.ClosePopupUI(this);
        }
    }
}
