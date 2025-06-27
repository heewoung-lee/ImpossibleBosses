using System;
using GameManagers;
using GameManagers.Interface.VivoxManager;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UILobbyChat : UIScene
    {
        [Inject] private ISendMessage _sendMessage;
        [Inject] private IVivoxSession _vivoxSession;
    
        enum Buttons
        {
            SendButton
        }
        enum InputFields
        {
            ChattingInputField
        }
        enum Texts
        {
            ChattingLog
        }
        enum ScrollRects
        {
            ChatScrollRect
        }

        private Button _sendButton;
        private TMP_InputField _chattingInputField;
        private TMP_Text _chatLog;
        private ScrollRect _chattingScrollRect;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Button>(typeof(Buttons));
            Bind<TMP_Text>(typeof(Texts));
            Bind<TMP_InputField>(typeof(InputFields));
            Bind<ScrollRect>(typeof(ScrollRects));
            _sendButton = Get<Button>((int)Buttons.SendButton);
            _chattingInputField = Get<TMP_InputField>((int)InputFields.ChattingInputField);
            _chatLog = Get<TMP_Text>((int)Texts.ChattingLog);
            _chattingScrollRect = Get<ScrollRect>((int)ScrollRects.ChatScrollRect);
            _sendButton.onClick.AddListener(() =>
            {
                SendChatingMessage(_chattingInputField.text);
            });
            _chattingInputField.onSubmit.AddListener(SendChatingMessage);
            _sendButton.interactable = false;
        }
        public void SendText(string text)
        {
            _chatLog.text += text;
            _chatLog.text += "\n";
        }


        protected override void StartInit()
        {
            base.StartInit();
            InitButtonInteractable();
            VivoxService.Instance.ChannelMessageReceived += ChannelMessageReceived;
        }

        private void InitButtonInteractable()
        {
            if (_vivoxSession.CheckDoneLoginProcess == false)
            {
                _vivoxSession.VivoxDoneLoginEvent += ButtonInteractable;
            }
            else
            {
                ButtonInteractable();
            }
        }

        public async void SendChatingMessage(string message)
        {
            if (string.IsNullOrEmpty(_chattingInputField.text) || _sendButton.interactable == false)
                return;

            try
            {
                await _sendMessage.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending message: {ex.Message}");
            }
            _chattingScrollRect.verticalNormalizedPosition = 0f;
            _chattingInputField.text = "";
            _chattingInputField.Select();
            _chattingInputField.ActivateInputField();
        }

        private void ChannelMessageReceived(VivoxMessage message)
        {
            string messageText = message.MessageText;
            _chatLog.text += $"{messageText} \n";
        }

        private void ButtonInteractable()
        {
            _sendButton.interactable = true;
        }

    }
}
