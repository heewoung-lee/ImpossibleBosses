using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyScene : UI_Scene
{
    UI_UserInfo_Panel _ui_User_Panel;
    UI_LobbyChat _ui_LobbyChat;
    UI_Room_Inventory _ui_Room_Inventory;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        _ui_User_Panel = Managers.UI_Manager.ShowSceneUI<UI_UserInfo_Panel>();
        _ui_LobbyChat = Managers.UI_Manager.ShowSceneUI<UI_LobbyChat>();
        _ui_Room_Inventory = Managers.UI_Manager.ShowSceneUI<UI_Room_Inventory>();
    }

 
    protected override void StartInit()
    {
        base.StartInit();
    }

}
