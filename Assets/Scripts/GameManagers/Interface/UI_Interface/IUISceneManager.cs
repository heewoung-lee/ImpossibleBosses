using System;
using System.Collections.Generic;
using UI.Scene;

namespace GameManagers.Interface.UI_Interface
{
    public interface IUISceneManager
    {
        public Dictionary<Type, UIScene> UISceneDict { get; }
        public T Get_Scene_UI<T>() where T : UIScene;
        public bool Try_Get_Scene_UI<T>(out T uiScene) where T : UIScene;

        public T GetSceneUIFromResource<T>(string name = null, string path = null) where T : UIScene;
        public T GetOrCreateSceneUI<T>(string name = null, string path = null) where T : UIScene;
    }
}
