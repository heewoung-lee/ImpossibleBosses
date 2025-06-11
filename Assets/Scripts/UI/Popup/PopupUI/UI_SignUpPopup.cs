using System;
using GameManagers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;

public class UI_SignUpPopup : ID_PW_Popup, IUI_HasCloseButton
{
    Button _buttonClose;
    Button _buttonSignup;
    TMP_InputField _idInputField;
    TMP_InputField _pwInputField;
    private UI_AlertPopupBase _alertPopup;
    private UI_AlertPopupBase _confirmPopup;

    public override TMP_InputField ID_Input_Field => _idInputField;

    public override TMP_InputField PW_Input_Field => _pwInputField;

    public Button CloseButton => _buttonClose;

    enum Buttons
    {
        Button_Close,
        Button_Signup
    }
    enum InputFields
    {
        IDInputField,
        PWInputField
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        _idInputField = Get<TMP_InputField>((int)InputFields.IDInputField);
        _pwInputField = Get<TMP_InputField>((int)InputFields.PWInputField);
        _buttonClose = Get<Button>((int)Buttons.Button_Close);
        _buttonSignup = Get<Button>((int)Buttons.Button_Signup);
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
            _alertPopup = ShowAlertDialogUI<UI_AlertDialog>(_alertPopup, "오류", message);
        }
        else
        {
            _confirmPopup = ShowAlertDialogUI<UI_ConfirmDialog>(_confirmPopup, "성공", message, ShowLoginAfterSignUp);
            ClearIDAndPW();
        }
    }

    public void ClearIDAndPW()
    {
        _idInputField.text = "";
        _pwInputField.text = "";
    }

    public void ShowLoginAfterSignUp()
    {
        Managers.UI_Manager.CloseAllPopupUI();
        UI_LoginPopup loginPopup = Managers.UI_Manager.GetImportant_Popup_UI<UI_LoginPopup>();
        Managers.UI_Manager.ShowPopupUI(loginPopup);
    }

    private UI_AlertPopupBase ShowAlertDialogUI<T>(UI_AlertPopupBase alertBasePopup,string titleMessage,string bodyText,UnityAction closeButtonAction = null) where T: UI_AlertPopupBase
    {
        if(alertBasePopup == null)
        {
            alertBasePopup = Managers.UI_Manager.TryGetPopupInDict<T>();
        }
        alertBasePopup.SetText(titleMessage, bodyText);
        if (closeButtonAction != null)
        {
            alertBasePopup.SetCloseButtonOverride(closeButtonAction);
        }
        Managers.UI_Manager.ShowPopupUI(alertBasePopup);

        return alertBasePopup;
    }

    protected override void StartInit()
    {
    }

    public void OnClickCloseButton()
    {
        Managers.UI_Manager.ClosePopupUI(this);
    }
}
