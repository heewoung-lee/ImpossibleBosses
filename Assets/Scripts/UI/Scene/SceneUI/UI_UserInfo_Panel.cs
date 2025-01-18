using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo_Panel : UI_Scene
{

    enum Buttons
    {
        CreateRoomButton,
        LoginSceneBackButton
    }

    enum Texts
    {
        PlayerNickNameText
    }


    Button _createRoomButton;
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
    private void InitButtonInteractable()
    {
        if (Managers.VivoxManager.CheckDoneLoginProcess == false)
        {
            Managers.VivoxManager.VivoxDoneLoginEvent -= ButtonInteractable;
            Managers.VivoxManager.VivoxDoneLoginEvent += ButtonInteractable;
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
            _createRoomUI = Managers.UI_Manager.ShowUIPopupUI<UI_CreateRoom>();
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
        await Managers.DisconnectApiEvent?.Invoke();
        Managers.SceneManagerEx.LoadScene(Define.Scene.LoginScene);
    }

    private void ButtonInteractable()
    {
        _createRoomButton.interactable = true;
        _loginSceneBackButton.interactable = true;
    }
    private void ButtonDisInteractable()
    {
        _createRoomButton.interactable = false;
        _loginSceneBackButton.interactable = false;
    }

}
