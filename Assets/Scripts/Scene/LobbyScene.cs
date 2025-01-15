using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override Define.Scene CurrentScene => Define.Scene.LobbyScene;
    UI_LobbyScene _uI_LobbyScene;
    NetworkManager _netWorkManager;
    UnityTransport _unityTransprot;

    UI_CreateRoom _ui_createRoom;
    public override void Clear()
    {
    }

    protected override void AwakeInit()
    {
    }

    protected override void StartInit()
    {
        base.StartInit();
        _uI_LobbyScene = Managers.UI_Manager.ShowSceneUI<UI_LobbyScene>();
        _ui_createRoom = Managers.UI_Manager.ShowUIPopupUI<UI_CreateRoom>();
        Managers.UI_Manager.ShowPopupUI(_ui_createRoom);
    }

}
