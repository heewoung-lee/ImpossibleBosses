using UnityEngine;
using UnityEngine.UI;

public class UI_SignUpPopup : UI_Popup
{
    Button _buttonClose;
    enum ButtonEvent
    {
        Button_Close,
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();

        Bind<Button>(typeof(ButtonEvent));
        _buttonClose = Get<Button>((int)ButtonEvent.Button_Close);
        _buttonClose.onClick.AddListener(() =>
        {
            Managers.UI_Manager.ClosePopupUI(this);
        });
    }


    protected override void StartInit()
    {
    }
}
