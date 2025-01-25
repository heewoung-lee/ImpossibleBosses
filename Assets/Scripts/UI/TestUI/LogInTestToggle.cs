using UnityEngine;
using UnityEngine.UI;

public class LogInTestToggle : UI_Scene
{

    enum Buttons
    {
        Test_Button
    }
    enum Toggles 
    {
        TestToggle
    }
    

    Button _testButton;
    Toggle _testToggle;
    protected override void AwakeInit()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Toggle>(typeof(Toggles));
        _testButton = Get<Button>((int)Buttons.Test_Button);
        _testToggle = Get<Toggle>((int)Toggles.TestToggle);
        _testButton.interactable = _testToggle.interactable;
        _testToggle.onValueChanged.AddListener((ison) =>
        {
            _testButton.gameObject.SetActive(ison);
        });
        _testButton.onClick.AddListener(ClickLogin);
    }

    private void ClickLogin()
    {
        UI_LoginPopup loginPopup = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_LoginPopup>();
        loginPopup.AuthenticateUser("hiwoong123", "123123");
    }

    protected override void StartInit()
    {
    }

}
