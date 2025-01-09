using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LoginPopup : UI_Popup
{

    enum Buttons
    {
        Close_Button,
        Signup_Button,
        Confirm_Button
    }

    enum InputFields
    {
        IDInputField,
        PWInputField
    }

    Button _close_Button;
    Button _signup_Button;
    Button _confirm_Button;
    UI_SignUpPopup _ui_signUpPopup;
    TMP_InputField _idInputField;
    TMP_InputField _pwInputField;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        _close_Button = Get<Button>((int)Buttons.Close_Button);
        _signup_Button = Get<Button>((int)Buttons.Signup_Button);
        _confirm_Button = Get<Button>((int)Buttons.Confirm_Button);
        _idInputField = Get<TMP_InputField>((int)InputFields.IDInputField);
        _pwInputField = Get<TMP_InputField>((int)InputFields.PWInputField);


        _close_Button.onClick.AddListener(() =>
        {
            Managers.UI_Manager.ClosePopupUI(this);
        });
        _signup_Button.onClick.AddListener(ShowSignUpUI);
        _confirm_Button.onClick.AddListener(AuthenticateUser);

    }
    protected override void StartInit()
    {
    }
    public void ShowSignUpUI()
    {
        if(_ui_signUpPopup == null)
        {
            _ui_signUpPopup = Managers.UI_Manager.ShowUIPopupUI<UI_SignUpPopup>();
        }
        Managers.UI_Manager.ShowPopupUI(_ui_signUpPopup);
    }

    public void AuthenticateUser()
    {
        string userID = _idInputField.text;
        string userPW = _pwInputField.text;

        Debug.Log("유저ID"+userID);
        Debug.Log("유저PW" + userPW);
        
    }

}
