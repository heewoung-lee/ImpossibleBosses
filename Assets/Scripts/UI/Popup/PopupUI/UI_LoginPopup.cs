using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Playmode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class UI_LoginPopup : ID_PW_Popup, IUI_HasCloseButton
{

    enum Buttons
    {
        Close_Button,
        Signup_Button,
        Confirm_Button
    }

    enum InputFields
    {
        IDInputField,
        PWInputField
    }

    Button _close_Button;
    Button _signup_Button;
    Button _confirm_Button;
    TMP_InputField _idInputField;
    TMP_InputField _pwInputField;
    UI_AlertPopupBase _ui_alertPopupUI;

    UI_CreateNickName _ui_CreateNickName;
    public UI_CreateNickName UI_CreateNickName
    {
        get
        {
            if (_ui_CreateNickName == null)
            {
                _ui_CreateNickName = Managers.UI_Manager.TryGetPopupInDict<UI_CreateNickName>();
            }
            return _ui_CreateNickName;
        }
    }

    public override TMP_InputField ID_Input_Field => _idInputField;

    public override TMP_InputField PW_Input_Field => _pwInputField;

    public Button CloseButton => _close_Button;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        _close_Button = Get<Button>((int)Buttons.Close_Button);
        _signup_Button = Get<Button>((int)Buttons.Signup_Button);
        _confirm_Button = Get<Button>((int)Buttons.Confirm_Button);
        _idInputField = Get<TMP_InputField>((int)InputFields.IDInputField);
        _pwInputField = Get<TMP_InputField>((int)InputFields.PWInputField);
        _close_Button.onClick.AddListener(OnClickCloseButton);
        _signup_Button.onClick.AddListener(ShowSignUpUI);
        _confirm_Button.onClick.AddListener(()=>AuthenticateUser(_idInputField.text, _pwInputField.text));
        Managers.UI_Manager.AddImportant_Popup_UI(this);
    }
    protected override void StartInit()
    {
    }
    public void ShowSignUpUI()
    {
        Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_SignUpPopup>();
    }

    private void OnDisable()
    {
        _idInputField.text = "";
        _pwInputField.text = "";
    }

    public async void AuthenticateUser(string userID,string userPW)
    {

        if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(userPW))
            return;

        _confirm_Button.interactable = false;

        if (Utill.IsAlphanumeric(userID) == false) //영문+숫자외 다른 문자가 섞인경우.
        {
            Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>()
                .AlertSetText("난 한글을 사랑하지만..", "아이디는 영문+숫자 조합으로만 쓸 수 있습니다.")
                .AfterAlertEvent(() => { _confirm_Button.interactable = true; });
            return;
        }

        try
        {
            PlayerLoginInfo playerinfo = Managers.LogInManager.AuthenticateUser(userID, userPW);

            if (playerinfo.Equals(default(PlayerLoginInfo)))
            {
                Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>()
                    .AlertSetText("오류", "아이디와 비밀번호가 틀립니다")
                    .AfterAlertEvent(() => { _confirm_Button.interactable = true; });
                return;
            }
            if (string.IsNullOrEmpty(playerinfo.NickName))
            {
                Managers.UI_Manager.ShowPopupUI(UI_CreateNickName);
                UI_CreateNickName.PlayerLoginInfo = playerinfo;
                _confirm_Button.interactable = true;
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error: {ex}\nNot Connetced Internet");
            UI_AlertPopupBase alertDialog = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>()
                .AlertSetText("오류", "인터넷 연결이 안됐습니다.")
                .AfterAlertEvent(()=> { _confirm_Button.interactable = true; });
            return;
        }
        Managers.SceneManagerEx.LoadSceneWithLoadingScreen(Define.Scene.LobbyScene);
        try
        {
            bool checkPlayerNickNameAlreadyConnected = await Managers.LobbyManager.InitLobbyScene();//로그인을 시도;
            if (checkPlayerNickNameAlreadyConnected is true)
            {
                Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>()
                  .AfterAlertEvent(() => { _confirm_Button.interactable = true; })
                  .AlertSetText("오류", "아이디가 이미 접속되어 있습니다.")
                 .AfterAlertEvent(() => Managers.SceneManagerEx.LoadScene(Define.Scene.LoginScene));
                LoadingSceneSetErrorOccurred();
                return;
            }
        }
        catch(Exception ex)
        {
            Debug.Log($"오류{ex}");
        }
    }

    public void OnClickCloseButton()
    {
        Managers.UI_Manager.ClosePopupUI(this);
    }

    private void LoadingSceneSetErrorOccurred()
    {

        if(Managers.SceneManagerEx.CurrentScene is LoadingScene loaingScene)
        {
            loaingScene.IsErrorOccurred = true;
        }
    }
}
