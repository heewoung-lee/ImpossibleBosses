using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.LoginManager;
using GameManagers.Interface.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.Popup.PopupUI
{
    public class UILoginPopup : IDPwPopup, IUIHasCloseButton
    {
        [Inject] private IPlayerLogininfo _playerLogininfo;
        [Inject] private IUIPopupManager _uiPopupManager;
        [Inject] private LobbyManager _lobbyManager;
        [Inject] SceneManagerEx _sceneManagerEx;
        enum Buttons
        {
            CloseButton,
            SignupButton,
            ConfirmButton
        }

        enum InputFields
        {
            IDInputField,
            PwInputField
        }

        private Button _closeButton;
        private Button _signupButton;
        private Button _confirmButton;
        private TMP_InputField _idInputField;
        private TMP_InputField _pwInputField;
        private UICreateNickName _uiCreateNickName;
        private UIAlertPopupBase _uiAlertPopupBase;

        public UICreateNickName UICreateNickName
        {
            get
            {
                if (_uiCreateNickName == null)
                {
                    _uiCreateNickName = _uiPopupManager.GetPopupInDict<UICreateNickName>();
                }

                return _uiCreateNickName;
            }
        }

        public override TMP_InputField IdInputField => _idInputField;

        public override TMP_InputField PwInputField => _pwInputField;

        public Button CloseButton => _closeButton;

        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Button>(typeof(Buttons));
            Bind<TMP_InputField>(typeof(InputFields));
            _closeButton = Get<Button>((int)Buttons.CloseButton);
            _signupButton = Get<Button>((int)Buttons.SignupButton);
            _confirmButton = Get<Button>((int)Buttons.ConfirmButton);
            _idInputField = Get<TMP_InputField>((int)InputFields.IDInputField);
            _pwInputField = Get<TMP_InputField>((int)InputFields.PwInputField);
            _closeButton.onClick.AddListener(OnClickCloseButton);
            _signupButton.onClick.AddListener(ShowSignUpUI);
            _confirmButton.onClick.AddListener(async () => await AuthenticateUser(_idInputField.text, _pwInputField.text));
            _uiPopupManager.AddImportant_Popup_UI(this);
        }

        protected override void StartInit()
        {
        }

        public void ShowSignUpUI()
        {
            if (_uiPopupManager.TryGetPopupDictAndShowPopup(out UISignUpPopup popup) == true)
            {
            }
        }

        private void OnDisable()
        {
            _idInputField.text = "";
            _pwInputField.text = "";
        }

        public async Task AuthenticateUser(string userID, string userPw)
        {
            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(userPw))
                return;

            _confirmButton.interactable = false;

            if (Utill.IsAlphanumeric(userID) == false) //영문+숫자외 다른 문자가 섞인경우.
            {
                if (_uiPopupManager.TryGetPopupDictAndShowPopup(out UIAlertDialog uiAlertDialog) == true)
                {
                    uiAlertDialog.AlertSetText("난 한글을 사랑하지만..", "아이디는 영문+숫자 조합으로만 쓸 수 있습니다.")
                        .AfterAlertEvent(() => { _confirmButton.interactable = true; });
                }

                return;
            }

            try
            {
                PlayerLoginInfo playerinfo = _playerLogininfo.FindAuthenticateUser(userID, userPw);

                if (playerinfo.Equals(default))
                {
                    if (_uiPopupManager.TryGetPopupDictAndShowPopup(out UIAlertDialog uiAlertDialog) == true)
                    {
                        uiAlertDialog.AlertSetText("오류", "아이디와 비밀번호가 틀립니다")
                            .AfterAlertEvent(() => { _confirmButton.interactable = true; });
                    }

                    return;
                }

                if (string.IsNullOrEmpty(playerinfo.NickName))
                {
                    _uiPopupManager.ShowPopupUI(UICreateNickName);
                    UICreateNickName.PlayerLoginInfo = playerinfo;
                    _confirmButton.interactable = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error: {ex}\nNot Connetced Internet");
                if (_uiPopupManager.TryGetPopupDictAndShowPopup(out UIAlertDialog dialog) == true)
                {
                    dialog.AlertSetText("오류", "인터넷 연결이 안됐습니다.")
                        .AfterAlertEvent(() => { _confirmButton.interactable = true; });
                }

                return;
            }

            _sceneManagerEx.LoadSceneWithLoadingScreen(Define.Scene.LobbyScene);
            try
            {
                bool checkPlayerNickNameAlreadyConnected = await _lobbyManager.InitLobbyScene(); //로그인을 시도;
                if (checkPlayerNickNameAlreadyConnected is true)
                {
                    if (_uiPopupManager.TryGetPopupDictAndShowPopup(out UIAlertDialog dialog) == true)
                    {
                        dialog.AfterAlertEvent(() => { _confirmButton.interactable = true; })
                            .AlertSetText("오류", "아이디가 이미 접속되어 있습니다.")
                            .AfterAlertEvent(() => _sceneManagerEx.LoadScene(Define.Scene.LoginScene));
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"오류{ex}");
                if (_uiPopupManager.TryGetPopupDictAndShowPopup(out UIAlertDialog dialog) == true)
                {
                    dialog.AfterAlertEvent(() => { _confirmButton.interactable = true; })
                        .AlertSetText("오류", "로그인중 문제가 생겼습니다.")
                        .AfterAlertEvent(() => _sceneManagerEx.LoadScene(Define.Scene.LoginScene));
                }

                return;
            }
            
        }

        public void OnClickCloseButton()
        {
            _uiPopupManager.ClosePopupUI(this);
        }
    }
}