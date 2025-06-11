using System;
using System.Linq;
using GameManagers;
using Unity.VisualScripting;
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

    enum ButtonImages
    {
        BackGroundSprite,
        InnerSprite
    }

    Button _extensionButton;
    RectTransform _backGroundGraphic;
    RectTransform _chatScrollRect;

    Image[] _buttonImages;
    Sprite[] _extensionImage;
    Sprite[] _contractionImage;
    bool isStateExtenstion = false;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        AddBind<Button>(typeof(AddButtons),out string[] indexString);
        Bind<RectTransform>((typeof(RectTransforms)));
        int extensionButtonIndex = Array.FindIndex(indexString, strings => strings == Enum.GetName(typeof(AddButtons), AddButtons.ExtensionButton));
        _extensionButton = Get<Button>(extensionButtonIndex);
        _backGroundGraphic = Get<RectTransform>((int)RectTransforms.BackgroundGraphic);
        _chatScrollRect = Get<RectTransform>((int)RectTransforms.ChatScrollRect);
        _extensionButton.onClick.AddListener(SetSwitchingChatting);
        _buttonImages = _extensionButton.gameObject.GetComponentsInChildren<Image>();
        _extensionImage = _extensionButton.gameObject.GetComponentsInChildren<Image>().Select(image => image.sprite).ToArray();
        _contractionImage = new Sprite[_extensionImage.Length];
        _contractionImage[(int)ButtonImages.BackGroundSprite] = Managers.ResourceManager.Load<Sprite>("Art/UI/ButtonImage/Button_Rectangle_Red");
        _contractionImage[(int)ButtonImages.InnerSprite] = Managers.ResourceManager.Load<Sprite>("Art/UI/ButtonImage/Icon_Minus");
    }

    protected override void StartInit()
    {
        base.StartInit();
    }

    private void SetSwitchingChatting()
    {
        if(isStateExtenstion == false)
        {
            _backGroundGraphic.offsetMax += Vector2.up * EXTENSION_SIZE;
            _chatScrollRect.offsetMax += Vector2.up * EXTENSION_SIZE;
            isStateExtenstion = true;
            _buttonImages[(int)ButtonImages.BackGroundSprite].sprite = _contractionImage[(int)ButtonImages.BackGroundSprite];
            _buttonImages[(int)ButtonImages.InnerSprite].sprite = _contractionImage[(int)ButtonImages.InnerSprite];
        }
        else
        {
            _backGroundGraphic.offsetMax -= Vector2.up * EXTENSION_SIZE;
            _chatScrollRect.offsetMax -= Vector2.up * EXTENSION_SIZE;
            isStateExtenstion = false;
            _buttonImages[(int)ButtonImages.BackGroundSprite].sprite = _extensionImage[(int)ButtonImages.BackGroundSprite];
            _buttonImages[(int)ButtonImages.InnerSprite].sprite = _extensionImage[(int)ButtonImages.InnerSprite];
        }
    }


}
