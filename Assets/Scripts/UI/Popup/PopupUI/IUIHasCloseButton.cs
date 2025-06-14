using UnityEngine.UI;

namespace UI.Popup.PopupUI
{
    internal interface IUIHasCloseButton
    {
        Button CloseButton { get; }
        public void OnClickCloseButton();
    }
}