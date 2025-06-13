using GameManagers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.PopupUI
{
    public class ClosePopupUIButton : MonoBehaviour
    {
        private Button _windowCloseButton;
    
        private UI_Popup _parentPopup;
        void Start()
        {
            _parentPopup = transform.FindParantComponent<UI_Popup>();

            _windowCloseButton = _windowCloseButton = Utill.FindChild(gameObject, "Button_Close", true).GetComponent<Button>();
            _windowCloseButton.onClick.AddListener(() =>
            {
                Managers.UIManager.ClosePopupUI(_parentPopup);
            });
        }

    }
}
