using UnityEngine;
using UnityEngine.UI;

public class UI_RoomChat : UI_LobbyChat
{
    private const int EXTENSION_SIZE = 400;
    enum AddButtons
    {
        ExtensionButton
    }

    enum RectTransforms
    {
        BackgroundGraphic,
        ChatScrollRect
    }

    Button _extensionButton;
    RectTransform _backGroundGraphic;
    RectTransform _chatScrollRect;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        AddBind<Button>(typeof(AddButtons));
        Bind<RectTransform>((typeof(RectTransforms)));
        _extensionButton = Get<Button>(1);
        //TODO: 인덱스 겹치는문제 해결해야함
        _backGroundGraphic = Get<RectTransform>((int)RectTransforms.BackgroundGraphic);
        _chatScrollRect = Get<RectTransform>((int)RectTransforms.ChatScrollRect);
        _extensionButton.onClick.AddListener(SetExtensionChatting);
    }

    protected override void StartInit()
    {
       // base.StartInit();
    }

    private void SetExtensionChatting()
    {
        _backGroundGraphic.offsetMax += Vector2.up * EXTENSION_SIZE;
        _chatScrollRect.offsetMax += Vector2.up * EXTENSION_SIZE;
    }


}
