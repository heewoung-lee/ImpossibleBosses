using UI;
using UI.Popup;
using UI.Scene;
using UnityEngine;

namespace GameManagers.Interface
{
    public interface IUIManager
    {
        public void SetCanvas(Canvas canvas, bool sorting = false);
        
        
        public T Get_Scene_UI<T>() where T : UIScene;
        public bool Try_Get_Scene_UI<T>(out T uiScene) where T : UIScene;

        public T GetSceneUIFromResource<T>(string name = null, string path = null) where T : UIScene;
        public T GetOrCreateSceneUI<T>(string name = null, string path = null) where T : UIScene;
        
        
        
        public T MakeUIWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UIBase;
        public T MakeSubItem<T>(Transform parent = null, string name = null, string path = null) where T : UIBase;
      
        
        public bool TryGetPopupInDict<T>() where T : UIPopup;
        public bool TryGetPopupDictAndShowPopup<T>() where T : UIPopup;
        public T GetPopupUIFromResource<T>(string name = null) where T : UIPopup;
        public void ShowPopupUI(UIPopup popup);
        public void ClosePopupUI();
        public void ClosePopupUI(UIPopup popup);
        public void CloseAllPopupUI();
        public void SwitchPopUpUI(UIPopup popup);
        public bool GetTopPopUpUI(UIPopup popup);
        
        public void AddImportant_Popup_UI(UIBase importantUI);
        
        public T GetImportant_Popup_UI<T>() where T : UIBase;
        
        
    }
}