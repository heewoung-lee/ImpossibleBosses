using System;
using System.Linq;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UIRoomChat : UILobbyChat
    {
        private const int ExtensionSize = 400;
        [Inject] IResourcesLoader _resourcesLoader;
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
        bool _isStateExtenstion = false;

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
            _contractionImage[(int)ButtonImages.BackGroundSprite] = _resourcesLoader.Load<Sprite>("Art/UI/ButtonImage/Button_Rectangle_Red");
            _contractionImage[(int)ButtonImages.InnerSprite] = _resourcesLoader.Load<Sprite>("Art/UI/ButtonImage/Icon_Minus");
        }

        protected override void StartInit()
        {
            base.StartInit();
        }

        private void SetSwitchingChatting()
        {
            if(_isStateExtenstion == false)
            {
                _backGroundGraphic.offsetMax += Vector2.up * ExtensionSize;
                _chatScrollRect.offsetMax += Vector2.up * ExtensionSize;
                _isStateExtenstion = true;
                _buttonImages[(int)ButtonImages.BackGroundSprite].sprite = _contractionImage[(int)ButtonImages.BackGroundSprite];
                _buttonImages[(int)ButtonImages.InnerSprite].sprite = _contractionImage[(int)ButtonImages.InnerSprite];
            }
            else
            {
                _backGroundGraphic.offsetMax -= Vector2.up * ExtensionSize;
                _chatScrollRect.offsetMax -= Vector2.up * ExtensionSize;
                _isStateExtenstion = false;
                _buttonImages[(int)ButtonImages.BackGroundSprite].sprite = _extensionImage[(int)ButtonImages.BackGroundSprite];
                _buttonImages[(int)ButtonImages.InnerSprite].sprite = _extensionImage[(int)ButtonImages.InnerSprite];
            }
        }


    }
}
