using System;
using System.Collections.Generic;
using UI.Popup;

namespace GameManagers.Interface.UIManager
{
    public interface IUIPopupManager
    {


        public T GetPopupInDict<T>() where T : UIPopup;
        public bool TryGetPopupDictAndShowPopup<T>(out T popup) where T : UIPopup;
        public T GetPopupUIFromResource<T>(string name = null) where T : UIPopup;
        public void ShowPopupUI(UIPopup popup);
        public void ClosePopupUI();
        public void ClosePopupUI(UIPopup popup);
        public void CloseAllPopupUI();
        public void SwitchPopUpUI(UIPopup popup);
        public bool GetTopPopUpUI(UIPopup popup);
        public void AddImportant_Popup_UI(UIPopup importantUI);
        public T GetImportant_Popup_UI<T>() where T : UIPopup;
    }
}
