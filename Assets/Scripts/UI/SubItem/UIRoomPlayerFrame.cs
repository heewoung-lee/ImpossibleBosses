using NetWork.NGO;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.SubItem
{
    public class UIRoomPlayerFrame : UIBase
    {
        enum Images
        {
            Bg
        }
        private readonly Color _emptyPlayerFrameColor = "#988B8B50".HexCodetoConvertColor();
        private CharacterSelectorNgo _characterNgo;
        private Image _bg;
        public CharacterSelectorNgo CharacterSelectorNgo { get => _characterNgo; }

        protected override void AwakeInit()
        {
            Bind<Image>(typeof(Images));
            _bg = Get<Image>((int)Images.Bg);
            _bg.color = _emptyPlayerFrameColor;
        }

        protected override void StartInit()
        {
        }

        public void SetCharacterSelector(GameObject chracterSelecter)
        {
            _characterNgo = chracterSelecter.GetComponent<CharacterSelectorNgo>();
        }


    
    }
}
