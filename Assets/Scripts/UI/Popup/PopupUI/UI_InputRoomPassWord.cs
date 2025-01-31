using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_InputRoomPassWord : UI_Popup
{
    enum InputFields
    {
        RoomPassWordInputField
    }
    enum Buttons
    {
        Confirm_Button
    }
    enum GameObjects
    {
        MessageError
    }
    private TMP_InputField _roomPwInputField;
    public TMP_InputField RoomPwInputField => _roomPwInputField;
    private Button _confirm_Button;
    private GameObject _messageError;
    private TMP_Text _errorMessageText;
    private Module_UI_FadeOut _errorMessageTextFadeOutMoudule;
    public PlayerLoginInfo PlayerLoginInfo { get; set; }

    protected override void OnDisableInit()
    {
        base.OnDisableInit();
        _roomPwInputField.text = "";
    }
    protected override void StartInit()
    {

    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _roomPwInputField = Get<TMP_InputField>((int)InputFields.RoomPassWordInputField);
        _confirm_Button = Get<Button>((int)Buttons.Confirm_Button);
        _messageError = Get<GameObject>((int)GameObjects.MessageError);
        _errorMessageText = _messageError.GetComponentInChildren<TMP_Text>();
        _errorMessageTextFadeOutMoudule = _messageError.GetComponent<Module_UI_FadeOut>();
        _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () => _confirm_Button.interactable = true;
        _messageError.SetActive(false);
    }
    public void ConfirmButtonAddListener(Func<Task<Lobby>> funcEvent)
    {
        Lobby lobby = null;
        _confirm_Button.onClick.AddListener(async () => lobby = await funcEvent());

        if (lobby == null)
        {
            _messageError.SetActive(true);
            _errorMessageText.text = "비밀번호가 틀렸습니다.";
            Managers.UI_Manager.ClosePopupUI(this);
        }
    }
}
