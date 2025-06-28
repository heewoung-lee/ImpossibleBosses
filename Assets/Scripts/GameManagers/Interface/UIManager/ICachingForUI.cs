using UI.Popup;
using UI.Scene;

namespace GameManagers.Interface.UIManager
{
    public interface ICachingForUI
    {
        public UIPopup CloseTopPopupUI();
        public void PushPopupUI(UIPopup ui);
        public int GetPopupStackCount();
        public UIPopup GetTopPopupUI();
        

        public bool TryGetSceneUI<T>(out UIScene sceneUI) where T : UIScene;
        public void AddSceneUI<T>(T sceneUI) where T : UIScene;
        public void OverWriteSceneUI<T>(T sceneUI) where T : UIScene;
        
        public bool TryGetPopupUI<T>(out UIPopup uiPopup) where T : UIPopup;
        public void AddImportantPopupUI<T>(T uiPopup) where T : UIPopup;
        public void OverWritePopupUI<T>(T uiPopup) where T : UIPopup;
        
    }
}
