using GameManagers;
using UI.Scene;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyScene : UIScene
{
    UI_UserInfo_Panel _ui_User_Panel;
    UI_LobbyChat _ui_LobbyChat;
    UI_Room_Inventory _ui_Room_Inventory;
    UI_LoadingPanel _ui_LoadingPanel;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        _ui_User_Panel = Managers.UIManager.GetSceneUIFromResource<UI_UserInfo_Panel>();
        _ui_LobbyChat = Managers.UIManager.GetSceneUIFromResource<UI_LobbyChat>();
        _ui_Room_Inventory = Managers.UIManager.GetSceneUIFromResource<UI_Room_Inventory>();
        _ui_LoadingPanel = Managers.UIManager.GetSceneUIFromResource<UI_LoadingPanel>();
    }


    protected override void StartInit()
    {
        base.StartInit();
    }


}
