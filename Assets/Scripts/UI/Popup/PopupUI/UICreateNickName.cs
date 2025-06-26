using GameManagers;
using GameManagers.Interface.LoginManager;
using GameManagers.Interface.UIManager;
using Module.UI_Module;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Popup.PopupUI
{
    public class UICreateNickName : UIPopup
    {
        [Inject] private IUIPopupManager _uiManager;
        [Inject] private IWriteGoogleSheet _writeGoogleSheet;
        
        enum InputFields
        {
            NickNameInputField
        }
        enum Buttons
        {
            ConfirmButton
        }
        enum GameObjects
        {
            MessageError
        }
        private TMP_InputField _nickNameInputField;
        private Button _confirmButton;
        private GameObject _messageError;
        private TMP_Text _errorMessageText;
        private ModuleUIFadeOut _errorMessageTextFadeOutMoudule;

        public PlayerLoginInfo PlayerLoginInfo { get; set; }

        protected override void OnDisableInit()
        {
            base.OnDisableInit();
            _nickNameInputField.text = "";
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
            _nickNameInputField = Get<TMP_InputField>((int)InputFields.NickNameInputField);
            _confirmButton = Get<Button>((int)Buttons.ConfirmButton);
            _messageError = Get<GameObject>((int)GameObjects.MessageError);
            _errorMessageText = _messageError.GetComponentInChildren<TMP_Text>();
            _errorMessageTextFadeOutMoudule = _messageError.GetComponent<ModuleUIFadeOut>();
            _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () => _confirmButton.interactable = true;
            _messageError.SetActive(false);
            _confirmButton.onClick.AddListener(CreateNickname);
        }

        public void CreateNickname()
        {
            _confirmButton.interactable = false;
            CreateUserNickName(PlayerLoginInfo, _nickNameInputField.text);
        }

        public async void CreateUserNickName(PlayerLoginInfo playerinfo, string nickname)
        {
            (bool isCheckResult, string message) = await _writeGoogleSheet.WriteNickNameToGoogleSheet(playerinfo, nickname);

            if (isCheckResult == false)
            {
                _messageError.SetActive(true);
                _errorMessageText.text = message;
            }
            else
            {
                _uiManager.ClosePopupUI(this);
            }
        }
    }
}
