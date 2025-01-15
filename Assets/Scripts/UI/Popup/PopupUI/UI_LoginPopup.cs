using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    UI_SignUpPopup _ui_signUpPopup;
    public UI_SignUpPopup UI_signUpPopup
    {
        get
        {
            if (_ui_signUpPopup == null)
            {
                _ui_signUpPopup = Managers.UI_Manager.ShowUIPopupUI<UI_SignUpPopup>();
            }
            return _ui_signUpPopup;
        }
    }
    public UI_AlertPopupBase UI_AlertPopupUI
    {
        get
        {
            if (_ui_alertPopupUI == null)
            {
                _ui_alertPopupUI = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            }
            return _ui_alertPopupUI;
        }
    }
    UI_CreateNickName _ui_CreateNickName;
    public UI_CreateNickName UI_CreateNickName
    {
        get
        {
            if (_ui_CreateNickName == null)
            {
                _ui_CreateNickName = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_CreateNickName>();
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
        _confirm_Button.onClick.AddListener(AuthenticateUser);
        Managers.UI_Manager.AddImportant_Popup_UI(this);
    }
    protected override void StartInit()
    {
    }
    public void ShowSignUpUI()
    {
        Managers.UI_Manager.ShowPopupUI(UI_signUpPopup);
    }

    private void OnDisable()
    {
        _idInputField.text = "";
        _pwInputField.text = "";
    }

    public void AuthenticateUser()
    {
        string userID = _idInputField.text;
        string userPW = _pwInputField.text;

        if (string.IsNullOrEmpty(userID)|| string.IsNullOrEmpty(userPW))
            return;

        if (Utill.IsAlphanumeric(userID) == false) //����+���ڿ� �ٸ� ���ڰ� ���ΰ��.
        {
            string titleText = "�� �ѱ��� ���������..";
            string bodyText = "���̵�� ����+���� �������θ� �� �� �ֽ��ϴ�.";
            UI_AlertPopupUI.SetText(titleText, bodyText);
            Managers.UI_Manager.ShowPopupUI(UI_AlertPopupUI);
            return;
        }


        PlayerLoginInfo playerinfo = Managers.LogInManager.AuthenticateUser(userID, userPW);

        if(playerinfo.Equals(default(PlayerLoginInfo)))
        {
            string titleText = "����";
            string bodyText = "���̵�� ��й�ȣ�� Ʋ���ϴ�";
            UI_AlertPopupUI.SetText(titleText, bodyText);
            Managers.UI_Manager.ShowPopupUI(UI_AlertPopupUI);
            Debug.Log("Login Failed");
            return;
        }

        if (string.IsNullOrEmpty(playerinfo.NickName))
        {
            Managers.UI_Manager.ShowPopupUI(UI_CreateNickName);
            UI_CreateNickName.PlayerLoginInfo = playerinfo;
            return;
        }

        Debug.Log("�α��οϷ�");
        Managers.SceneManagerEx.LoadSceneWithLoadingScreen(Define.Scene.LobbyScene);
    }

    public void OnClickCloseButton()
    {
        Managers.UI_Manager.ClosePopupUI(this);
    }
}
