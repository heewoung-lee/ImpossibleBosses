using System;
using System.Collections.Generic;
using UI;
using UI.Popup;
using UI.Scene;

namespace GameManagers.Interface
{
    public interface IUIPopupManager
    {
        public Dictionary<Type, UIPopup> ImportantPopupUI { get; }
        public Stack<UIPopup> UIPopupStack { get; }


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
