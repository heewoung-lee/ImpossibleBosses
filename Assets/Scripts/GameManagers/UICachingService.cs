using System;
using System.Collections.Generic;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
using Scene.ZenjectInstaller;
using UI.Popup;
using UI.Scene;
using UnityEngine;
using Zenject;

namespace GameManagers
{
    public class UICachingService: ICachingForUI,IDisposable
    {
        private readonly IRegistrar<ICachingForUI> _registrar;
        
        private Stack<UIPopup> _uiPopups = new Stack<UIPopup>();
        private Dictionary<Type, UIPopup> _importantPopupUI = new Dictionary<Type, UIPopup>();
        private Dictionary<Type, UIScene> _uiSceneDict = new Dictionary<Type, UIScene>();
        [Inject]
        public UICachingService(IRegistrar<ICachingForUI> registrar)
        {
            _registrar = registrar;
            _registrar.Register(this);
        }
        public void Dispose()
        {
            _registrar.Unregister(this);
        }
        public UIPopup CloseTopPopupUI()
        {
            return _uiPopups.Pop();
        }
        public void PushPopupUI(UIPopup ui)
        {
            _uiPopups.Push(ui);
        }
        public int GetPopupStackCount()
        {
            return _uiPopups.Count;
        }
        public UIPopup GetTopPopupUI()
        {
            return _uiPopups.Peek();
        }

        public bool TryGetPopupUI<T>(out UIPopup uiPopup) where T : UIPopup
        {
            uiPopup = null;
            if (_importantPopupUI.TryGetValue(typeof(T), out UIPopup getUIPopup) != true) return false;
            uiPopup = getUIPopup;
            return true;
        }

        public void AddImportantPopupUI<T>(T uiPopup) where T : UIPopup
        {
            _importantPopupUI.Add(typeof(T), uiPopup);
        }

        public void OverWritePopupUI<T>(T uiPopup) where T : UIPopup
        {
            _importantPopupUI[typeof(T)] = uiPopup;
        }

        public bool TryGetSceneUI<T>(out UIScene sceneUI) where T : UIScene
        {
            sceneUI = null;
            if (_uiSceneDict.TryGetValue(typeof(T),out UIScene getSceneUI) != true) return false;
            sceneUI = getSceneUI;
            return true;
        }

        public void AddSceneUI<T>(T sceneUI) where T : UIScene
        {
            _uiSceneDict.Add(typeof(T), sceneUI);
        }

        public void OverWriteSceneUI<T>(T sceneUI) where T : UIScene
        {
           _uiSceneDict[typeof(T)] = sceneUI;
        }

        


    }
}
