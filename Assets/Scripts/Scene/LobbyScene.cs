using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Vivox;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override Define.Scene CurrentScene => Define.Scene.LobbyScene;
    UI_LobbyScene _uI_LobbyScene;
    public override void Clear()
    {
    }

    protected override void AwakeInit()
    {
    }

    protected override void StartInit()
    {
        base.StartInit();
        _uI_LobbyScene = Managers.UI_Manager.GetSceneUIFromResource<UI_LobbyScene>();

    }
    private async void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "GetJoinCode"))
        {
            Debug.Log($"�� �����ڵ�� {Managers.RelayManager.JoinCode}");
            Debug.Log($"�κ��� �����ڵ��{(await Managers.LobbyManager.GetCurrentLobby()).Data["RelayCode"].Value}");
        }
    }
}
