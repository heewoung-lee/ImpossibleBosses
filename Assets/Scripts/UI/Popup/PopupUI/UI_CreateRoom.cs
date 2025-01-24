using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateRoom : ID_PW_Popup, IUI_HasCloseButton
{

    enum InputFields
    {
        RoomNameInputField,
        RoomPWInputField
    }

    enum Buttons
    {
        Button_Close,
        Button_Connect
    }

    enum Sliders { UserCountSlider }
    enum Texts { CurrentCount }


    private TMP_InputField _roomNameInputField;
    private TMP_InputField _roomPWInputField;

    private Button _button_close;
    private Button _button_connect;

    private Slider _userCount_slider;
    private TMP_Text _currentCount;

    public override TMP_InputField ID_Input_Field => _roomNameInputField;
    public override TMP_InputField PW_Input_Field => _roomPWInputField;

    public Button CloseButton => _button_close;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        Bind<TMP_Text>(typeof(Texts));
        _roomNameInputField = Get<TMP_InputField>((int)InputFields.RoomNameInputField);
        _roomPWInputField = Get<TMP_InputField>((int) InputFields.RoomPWInputField);
        _button_connect = Get<Button>((int)Buttons.Button_Connect);
        _button_close = Get<Button>((int)Buttons.Button_Close);
        _userCount_slider = Get<Slider>((int)Sliders.UserCountSlider);
        _currentCount = Get<TMP_Text>((int)Texts.CurrentCount);
        _button_connect.onClick.AddListener(ConnectRoom);
        _button_close.onClick.AddListener(OnClickCloseButton);
        _userCount_slider.onValueChanged.AddListener((value) =>
        {
            _currentCount.text = value.ToString();
        });
    }

    public async void ConnectRoom()
    {
        try
        {
            if (string.IsNullOrEmpty(_roomNameInputField.text))
                return;

            string passWord = _roomPWInputField.text;
            int value = 0;
            CreateLobbyOptions option;


            if (string.IsNullOrEmpty(passWord) == false)
            {
                value = int.Parse(passWord);
                if((float)value / 10000000 < 1)
                {
                    UI_AlertPopupBase alertDialog = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>()
                   .AlertSetText("오류", "비밀번호는 8자리 이상");
                    return;
                }
                option = new CreateLobbyOptions()
                {
                    IsPrivate = false,
                    Password = passWord
                };
            }
            else
            {
                option = new CreateLobbyOptions()
                { IsPrivate = false };
            }
            Lobby robby = await Managers.LobbyManager.CreateLobby(ID_Input_Field.text, int.Parse(_currentCount.text), option);
            Managers.SceneManagerEx.LoadScene(Define.Scene.RoomScene);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
      
    }
    public void OnClickCloseButton()
    {
        Managers.UI_Manager.ClosePopupUI(this);
    }
    protected override void StartInit()
    {
    }
}
