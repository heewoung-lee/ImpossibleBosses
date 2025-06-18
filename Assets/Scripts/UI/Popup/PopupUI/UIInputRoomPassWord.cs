using System;
using System.Threading.Tasks;
using GameManagers;
using Module.UI_Module;
using TMPro;
using UI.SubItem;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.Popup.PopupUI
{
    public class UIInputRoomPassWord : UIPopup
    {
        enum InputFields
        {
            RoomPassWordInputField
        }

        enum Buttons
        {
            ConfirmButton
        }

        enum GameObjects
        {
            MessageError
        }

        private TMP_InputField _roomPwInputField;
        private UIRoomInfoPanel _roomInfoPanel;
        private Button _confirmButton;
        private GameObject _messageError;
        private TMP_Text _errorMessageText;
        private ModuleUIFadeOut _errorMessageTextFadeOutMoudule;
        [Inject] private UIManager _uiManager;

        public PlayerLoginInfo PlayerLoginInfo { get; set; }


        protected override void OnDisableInit()
        {
            base.OnDisableInit();
            _roomPwInputField.text = "";
            if (_roomInfoPanel != null)
            {
                _roomInfoPanel.JoinButtonInteractive(true);
            }
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
            _confirmButton = Get<Button>((int)Buttons.ConfirmButton);
            _messageError = Get<GameObject>((int)GameObjects.MessageError);
            _errorMessageText = _messageError.GetComponentInChildren<TMP_Text>();
            _errorMessageTextFadeOutMoudule = _messageError.GetComponent<ModuleUIFadeOut>();
            _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () => _confirmButton.interactable = true;
            _confirmButton.onClick.AddListener(async () => await CheckJoinRoom());
            _messageError.SetActive(false);
        }

        public void SetRoomInfoPanel(UIRoomInfoPanel infoPanel)
        {
            _roomInfoPanel = infoPanel;
        }

        private async Task CheckJoinRoom()
        {
            _confirmButton.interactable = false;
            Lobby lobby = _roomInfoPanel.LobbyRegisteredPanel;
            try
            {
                await Managers.LobbyManager.LoadingPanel(async () =>
                {
                    await Managers.LobbyManager.JoinLobbyByID(lobby.Id, _roomPwInputField.text);
                });
            }
            catch (Unity.Services.Lobbies.LobbyServiceException wrongPw) when
                (wrongPw.Reason == Unity.Services.Lobbies.LobbyExceptionReason.IncorrectPassword ||
                 wrongPw.Reason == LobbyExceptionReason.ValidationError)
            {
                _errorMessageText.text = "비밀번호가 틀렸습니다";
                _messageError.SetActive(true);
                _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () => { _confirmButton.interactable = true; };
                return;
            }
            catch (LobbyServiceException notfound) when (notfound.Reason == LobbyExceptionReason.LobbyNotFound)
            {
                Debug.Log("로비를 찾을 수 없습니다");

                if (_uiManager.TryGetPopupDictAndShowPopup(out UIAlertDialog loginPopup) == true)
                {
                    loginPopup.AlertSetText("오류", "로비를 찾을 수 없습니다")
                        .AfterAlertEvent(async () =>
                        {
                            _uiManager.ClosePopupUI(this);
                            _confirmButton.interactable = true;
                            await Managers.LobbyManager.ReFreshRoomList();
                        });
                }
                return;
            }
            catch (Exception error)
            {
                Debug.Log($"에러가 발생했습니다{error}");
                return;
            }
            finally
            {
                Managers.LobbyManager.TriggerLobbyLoadingEvent(false);
            }


            Managers.SceneManagerEx.LoadScene(Define.Scene.RoomScene);
        }
    }
}