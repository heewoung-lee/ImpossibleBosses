using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using UnityEngine;
using UnityEngine.UI;

public class UI_LoginTitle : UI_Scene
{
    enum ButtonEvent
    {
        Button_Start,
    }


    Button _openLoginButton;
    UI_LoginPopup _loginPopup;
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
        if(_loginPopup == null)
        {
            _loginPopup = Managers.UI_Manager.GetPopupUIFromResource<UI_LoginPopup>();
        }
        Managers.UI_Manager.ShowPopupUI(_loginPopup);
    }


}
