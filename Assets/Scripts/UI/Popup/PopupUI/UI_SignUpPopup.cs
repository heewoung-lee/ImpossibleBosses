using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SignUpPopup : UI_Popup
{
    Button _buttonClose;
    Button _buttonSignup;
    TMP_InputField _idInputField;
    TMP_InputField _pwInputField;
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
        _buttonClose.onClick.AddListener(() =>
        {
            Managers.UI_Manager.ClosePopupUI(this);
        });
        _buttonSignup.onClick.AddListener(CreateID);
    }
    public async void CreateID()
    {
      await Managers.LogInManager.WriteToGoogleSheet(_idInputField.text,_pwInputField.text);
    }

    protected override void StartInit()
    {
    }
}
