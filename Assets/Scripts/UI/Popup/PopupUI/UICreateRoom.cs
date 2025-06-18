using System;
using GameManagers;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Zenject;
using Input = UnityEngine.Input;

namespace UI.Popup.PopupUI
{
    public class UICreateRoom : IDPwPopup, IUIHasCloseButton
    {

        enum InputFields
        {
            RoomNameInputField,
            RoomPwInputField
        }

        enum Buttons
        {
            ButtonClose,
            ButtonConnect
        }

        enum Sliders { UserCountSlider }
        enum Texts { CurrentCount }


        private TMP_InputField _roomNameInputField;
        private TMP_InputField _roomPwInputField;

        private Button _buttonClose;
        private Button _buttonConnect;

        private Slider _userCountSlider;
        private TMP_Text _currentCount;
        [Inject] private UIManager _uiManager;

        
        public override TMP_InputField IdInputField => _roomNameInputField;
        public override TMP_InputField PwInputField => _roomPwInputField;

        public Button CloseButton => _buttonClose;


        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<Button>(typeof(Buttons));
            Bind<Slider>(typeof(Sliders));
            Bind<TMP_Text>(typeof(Texts));
            _roomNameInputField = Get<TMP_InputField>((int)InputFields.RoomNameInputField);
            _roomPwInputField = Get<TMP_InputField>((int)InputFields.RoomPwInputField);
            _buttonConnect = Get<Button>((int)Buttons.ButtonConnect);
            _buttonClose = Get<Button>((int)Buttons.ButtonClose);
            _userCountSlider = Get<Slider>((int)Sliders.UserCountSlider);
            _currentCount = Get<TMP_Text>((int)Texts.CurrentCount);
            _buttonConnect.onClick.AddListener(ConnectRoom);
            _buttonClose.onClick.AddListener(OnClickCloseButton);
            _userCountSlider.onValueChanged.AddListener((value) =>
            {
                _currentCount.text = value.ToString();
            });

            _roomNameInputField.onEndEdit.AddListener((value) =>
            {
                string finalText = value;
                if (!string.IsNullOrEmpty(Input.compositionString))
                {
                    finalText += Input.compositionString;
                }
                _roomNameInputField.text = finalText;
            });
        }

        public async void ConnectRoom()
        {
            _buttonConnect.interactable = false;
            try
            {
                if (string.IsNullOrEmpty(_roomNameInputField.text))
                    return;

                string passWord = _roomPwInputField.text;
                int value = 0;
                CreateLobbyOptions option;


                if (string.IsNullOrEmpty(passWord) == false)
                {
                    value = int.Parse(passWord);
                    if ((float)value / 10000000 < 1)
                    {
                        if (_uiManager.TryGetPopupDictAndShowPopup(out UIAlertDialog dialog) == true)
                        {
                            dialog .AlertSetText("오류", "비밀번호는 8자리 이상");
                            _buttonConnect.interactable = true;
                        }
                        return;
                    }

                    Debug.Log($"비밀번호가 있음{value}");
                    option = new CreateLobbyOptions()
                    {
                        IsPrivate = false,
                        Password = passWord,
                        Data = new System.Collections.Generic.Dictionary<string, DataObject>
                        {
                            {"LobbyType",new DataObject(DataObject.VisibilityOptions.Public,value:"CharactorSelect",index:DataObject.IndexOptions.S1) }
                        }
                    };
                }
                else
                {
                    option = new CreateLobbyOptions()
                    {
                        IsPrivate = false,
                        Data = new System.Collections.Generic.Dictionary<string, DataObject>
                        {
                            {"LobbyType",new DataObject(DataObject.VisibilityOptions.Public,value:"CharactorSelect",index:DataObject.IndexOptions.S1) }
                        }
                    };
                }
                await Managers.LobbyManager.LoadingPanel(async () =>
                {
                    await Managers.LobbyManager.CreateLobby(_roomNameInputField.text, int.Parse(_currentCount.text), option);
                    Managers.SceneManagerEx.LoadScene(Define.Scene.RoomScene);
                });
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _buttonConnect.interactable = true;
            }

        }
        public void OnClickCloseButton()
        {
            _uiManager.ClosePopupUI(this);
        }
        protected override void StartInit()
        {
        }
    }
}
