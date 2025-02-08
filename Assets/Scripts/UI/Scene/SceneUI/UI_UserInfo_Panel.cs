using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo_Panel : UI_Scene
{

    enum Buttons
    {
        CreateRoomButton,
        RefreshLobbyButton,
        LoginSceneBackButton
    }

    enum Texts
    {
        PlayerNickNameText
    }


    Button _createRoomButton;
    Button _refreshLobbyButton;
    Button _loginSceneBackButton;
    UI_CreateRoom _createRoomUI;

    TMP_Text _userNickNamaText;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        _createRoomButton = Get<Button>((int)Buttons.CreateRoomButton);
        _createRoomButton.onClick.AddListener(ShowCreateRoomUI);
        _refreshLobbyButton = Get<Button>((int)Buttons.RefreshLobbyButton);
        _refreshLobbyButton.onClick.AddListener(RefreshButton);
        _loginSceneBackButton = Get<Button>((int)Buttons.LoginSceneBackButton);
        _loginSceneBackButton.onClick.AddListener(MoveLoginScene);
        _userNickNamaText = Get<TMP_Text>((int)Texts.PlayerNickNameText);
        ButtonDisInteractable();
        ShowUserNickName();
    }

    protected override void StartInit()
    {
        base.StartInit();
        InitButtonInteractable();
    }

    public async void RefreshButton()
    {
        _refreshLobbyButton.interactable = false;
        UI_Room_Inventory inventory = Managers.UI_Manager.Get_Scene_UI<UI_Room_Inventory>();
        try
        {
            await Managers.LobbyManager.ReFreshRoomList();
            await Managers.LobbyManager.ShowUpdatedLobbyPlayers();
        }
        catch (Exception ex)
        {
            UI_AlertDialog alert_Popup =  Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alert_Popup.SetText("����", $"{ex}");
            _refreshLobbyButton.interactable = true;
        }
        _refreshLobbyButton.interactable = true;
        GetActiveVivoxChannels();
    }


    public void GetActiveVivoxChannels()
    {
        var activeChannels = VivoxService.Instance.ActiveChannels;

        if (activeChannels.Count == 0)
        {
            Debug.Log("���� ���� ���� ä���� �����ϴ�.");
            return;
        }

        Debug.Log($"���� ���� ���� VIVOX ä�� ��: {activeChannels.Count}");

        foreach (var channel in activeChannels)
        {
            string channelName = channel.Key; // ä�� ID �Ǵ� �̸�
            var channelSession = channel.Value; // ä�� ���� ����

            Debug.Log($"ä�� �̸�: {channelName}");
        }
    }
    private void InitButtonInteractable()
    {
        if (Managers.LobbyManager.IsDoneInitEvent == false)
        {
            Managers.LobbyManager.InitDoneEvent -= ButtonInteractable;
            Managers.LobbyManager.InitDoneEvent += ButtonInteractable;
        }
        else
        {
            ButtonInteractable();
        }
    }
    public void ShowCreateRoomUI()
    {
        if (_createRoomUI == null)
        {
            _createRoomUI = Managers.UI_Manager.GetPopupUIFromResource<UI_CreateRoom>();
        }
        Managers.UI_Manager.ShowPopupUI(_createRoomUI);
    }

    private void ShowUserNickName()
    {
        if (Managers.LobbyManager.CurrentPlayerInfo.Equals(default(PlayerIngameLoginInfo)))
        {
            Managers.LobbyManager.InitDoneEvent += ShowNickname;
        }
        else
        {
            ShowNickname();
        }
    }

    private void ShowNickname() 
    {
        _userNickNamaText.text += Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
    }

    public async void MoveLoginScene()
    {
        try
        {
            await Managers.SocketEventManager.DisconnectApiEvent.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log($"������ �߻��߽��ϴ�.{e}");
            return;
        }
        Managers.SceneManagerEx.LoadScene(Define.Scene.LoginScene);
    }

    private void ButtonInteractable()
    {
        _createRoomButton.interactable = true;
        _refreshLobbyButton.interactable = true;
        _loginSceneBackButton.interactable = true;
    }
    private void ButtonDisInteractable()
    {
        _createRoomButton.interactable = false;
        _refreshLobbyButton.interactable = false;
        _loginSceneBackButton.interactable = false;
    }

}
