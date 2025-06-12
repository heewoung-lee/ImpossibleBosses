using System;
using System.Threading.Tasks;
using GameManagers;
using Module.UI_Module;
using TMPro;
using Unity.Services.Lobbies;
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
    private UI_Room_Info_Panel _roomInfoPanel;
    private Button _confirm_Button;
    private GameObject _messageError;
    private TMP_Text _errorMessageText;
    private ModuleUIFadeOut _errorMessageTextFadeOutMoudule;

    public PlayerLoginInfo PlayerLoginInfo { get; set; }


    protected override void OnDisableInit()
    {
        base.OnDisableInit();
        _roomPwInputField.text = "";
        _roomInfoPanel = null;
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
        _errorMessageTextFadeOutMoudule = _messageError.GetComponent<ModuleUIFadeOut>();
        _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () => _confirm_Button.interactable = true;
        _confirm_Button.onClick.AddListener(async () => await CheckJoinRoom());
        _messageError.SetActive(false);
    }
    public void SetRoomInfoPanel(UI_Room_Info_Panel info_panel)
    {
        _roomInfoPanel = info_panel;
    }
    private async Task CheckJoinRoom()
    {
        _confirm_Button.interactable = false;
        Lobby lobby = _roomInfoPanel.LobbyRegisteredPanel;
        try
        {
            await Managers.LobbyManager.LoadingPanel(async() =>
            {
                await Managers.LobbyManager.JoinLobbyByID(lobby.Id, _roomPwInputField.text);
            });
        }
        catch (Unity.Services.Lobbies.LobbyServiceException wrongPw) when (wrongPw.Reason == Unity.Services.Lobbies.LobbyExceptionReason.IncorrectPassword)
        {
            _errorMessageText.text = "비밀번호가 틀렸습니다";
            _messageError.SetActive(true);
            _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () =>
            {
                _confirm_Button.interactable = true;
            };
            return;
        }
        catch (LobbyServiceException notfound) when (notfound.Reason == LobbyExceptionReason.LobbyNotFound)
        {
            Debug.Log("로비를 찾을 수 없습니다");
            Managers.UIManager.TryGetPopupDictAndShowPopup<UI_AlertDialog>().AlertSetText("오류", "로비를 찾을 수 없습니다")
                .AfterAlertEvent(async () =>
                {
                    Managers.UIManager.ClosePopupUI(this);
                    _confirm_Button.interactable = true;
                    await Managers.LobbyManager.ReFreshRoomList();
                });
            return;
        }
        catch (Exception error)
        {
            Debug.Log($"에러가 발생했습니다{error}");
            return;
        }
        Managers.SceneManagerEx.LoadScene(Define.Scene.RoomScene);
    }
}
