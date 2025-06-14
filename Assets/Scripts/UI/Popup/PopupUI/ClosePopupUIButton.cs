using GameManagers;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Popup.PopupUI
{
    public class ClosePopupUIButton : MonoBehaviour
    {
        private Button _windowCloseButton;
    
        private UIPopup _parentPopup;
        void Start()
        {
            _parentPopup = transform.FindParantComponent<UIPopup>();

            _windowCloseButton = _windowCloseButton = Utill.FindChild(gameObject, "Button_Close", true).GetComponent<Button>();
            _windowCloseButton.onClick.AddListener(() =>
            {
                Managers.UIManager.ClosePopupUI(_parentPopup);
            });
        }

    }
}
