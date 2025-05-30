using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
public class UI_LobbyChat : UI_Scene
{
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
        if (Managers.VivoxManager.CheckDoneLoginProcess == false)
        {
            Managers.VivoxManager.VivoxDoneLoginEvent += ButtonInteractable;
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
            await Managers.VivoxManager.SendMessageAsync(message);
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
        string senderID = message.SenderPlayerId;
        string senderDisplayName = message.SenderDisplayName;
        string messageChannel = message.ChannelName;

        _chatLog.text += $"{messageText} \n";
    }

    private void ButtonInteractable()
    {
        _sendButton.interactable = true;
    }

}
