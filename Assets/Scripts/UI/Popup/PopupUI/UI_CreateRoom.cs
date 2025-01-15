using TMPro;
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

    public void ConnectRoom()
    {

    }
    public void OnClickCloseButton()
    {
        Managers.UI_Manager.ClosePopupUI(this);
    }
    protected override void StartInit()
    {
    }
}
