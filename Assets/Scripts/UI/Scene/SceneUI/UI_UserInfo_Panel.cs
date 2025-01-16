using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo_Panel : UI_Scene
{
    enum Buttons
    {
        CreateRoomButton,
        LoginSceneBackButton
    }

    Button _createRoomButton;
    Button _loginSceneBackButton;
    UI_CreateRoom _createRoomUI;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Button>(typeof(Buttons));
        _createRoomButton = Get<Button>((int)Buttons.CreateRoomButton);
        _createRoomButton.onClick.AddListener(ShowCreateRoomUI);
        _loginSceneBackButton = Get<Button>((int)Buttons.LoginSceneBackButton);
    }

    public void ShowCreateRoomUI()
    {
        if (_createRoomUI == null)
        {
            _createRoomUI = Managers.UI_Manager.ShowUIPopupUI<UI_CreateRoom>();
        }
        Managers.UI_Manager.ShowPopupUI(_createRoomUI);
    }

}
