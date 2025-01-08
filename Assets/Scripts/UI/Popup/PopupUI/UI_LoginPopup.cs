using UnityEngine;
using UnityEngine.UI;

public class UI_LoginPopup : UI_Popup
{

    enum ButtonEvent
    {
        Button_Close,
        Button_Signup
    }

    Button _close_Button;
    Button _signup_Button;
    UI_SignUpPopup _ui_signUpPopup;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Button>(typeof(ButtonEvent));
        _close_Button = Get<Button>((int)ButtonEvent.Button_Close);
        _signup_Button = Get<Button>((int)ButtonEvent.Button_Signup);
        _close_Button.onClick.AddListener(() =>
        {
            Managers.UI_Manager.ClosePopupUI(this);
        });
        _signup_Button.onClick.AddListener(ShowSignUpUI);

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


}
