using System;
using System.Collections.Generic;
using GameManagers.Interface;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using UI;
using UI.Popup;
using UI.Popup.PopupUI;
using UI.Scene;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    internal class UIManager : IUIManager,IUIPopupManager,IUISceneManager,IUISubItem
    {
        
        [Inject] private IInstantiate _instantiate;
        private const int SceneUISortingDefaultValue = 0;
        private const int PopupUISortingDefaultValue = 20;
        
        int _sorting = SceneUISortingDefaultValue;
        int _popupSorting = PopupUISortingDefaultValue;
        
        private Stack<UIPopup> _uiPopups = new Stack<UIPopup>();
        private Dictionary<Type, UIScene> _uiSceneDict = new Dictionary<Type, UIScene>();
        private Dictionary<Type, UIPopup> _importantPopupUI = new Dictionary<Type, UIPopup>();
        
        public GameObject Root
        {
            get
            {
                GameObject go = GameObject.Find("@UI_ROOT");
                if (go == null)
                {
                    go = new GameObject() { name = "@UI_ROOT" };
                }
                return go;
            }
        }
        public void AddImportant_Popup_UI(UIPopup importantUI)
        {
            Type type = importantUI.GetType();
            _importantPopupUI[type] = importantUI;
        }

        public T GetImportant_Popup_UI<T>() where T : UIPopup
        {

            if (_importantPopupUI.TryGetValue(typeof(T), out UIPopup value))
            {
                return value as T;
            }

            Debug.Log($"Not Found KeyType: {typeof(T)}");
            return null;
        }

        public T Get_Scene_UI<T>() where T : UIScene
        {

            if (_uiSceneDict.TryGetValue(typeof(T), out UIScene value))
            {
                return value as T;
            }

            Debug.LogError($"Not Found KeyType: {typeof(T)}");
            return null;
        }

        public bool Try_Get_Scene_UI<T>(out T uiScene) where T : UIScene
        {
            if (_uiSceneDict.TryGetValue(typeof(T), out UIScene scene))
            {
                uiScene = scene as T;
                return uiScene is not null;
            }
            uiScene = null;
            return false;
        }
        public void SetCanvas(Canvas canvas, bool sorting = false)//씬 넘어갈때 다초기화 할것
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            if (sorting)
            {
                if (canvas.GetComponent<UIPopup>() != null)
                {
                    _popupSorting++;
                    canvas.sortingOrder = _popupSorting;
                }
                else
                {
                    _sorting++;
                    canvas.sortingOrder = _sorting;
                }
            }
            else
            {
                canvas.sortingOrder = 0;
            }
        }


        public T GetPopupInDict<T>() where T : UIPopup
        {
            T popup = GetImportant_Popup_UI<T>();
            if (popup == null)
            {
                popup = GetPopupUIFromResource<T>();
                AddImportant_Popup_UI(popup);
            }
            return popup;
        }

        public bool TryGetPopupDictAndShowPopup<T>(out T uiPopup) where T : UIPopup
        {
            uiPopup = GetPopupInDict<T>();
            if (uiPopup != null)
            {
                ShowPopupUI(uiPopup);
                return true;
            }
            return false;
        }

        public T GetPopupUIFromResource<T>(string name = null) where T : UIPopup
        {
            if (name == null)
                name = typeof(T).Name;

            GameObject go = _instantiate.InstantiateByPath($"Prefabs/UI/Popup/{name}");
            T popup = _instantiate.GetOrAddComponent<T>(go);

            //_ui_Popups.Push(popup);
            //ShowPopupUI(popup);

            go.transform.SetParent(Root.transform);

            if(popup is IPopupHandler handler)
            {
                handler.ClosePopup();
            }
            else
            {
                popup.gameObject.SetActive(false);
            }
            return popup;
        }
        public T GetSceneUIFromResource<T>(string name = null, string path = null) where T : UIScene
        {
            if (name == null)
                name = typeof(T).Name;

            GameObject go = null;
            if (path == null)
            {
                go = _instantiate.InstantiateByPath($"Prefabs/UI/MainUI/{name}");
            }
            else
            {
                go = _instantiate.InstantiateByPath($"{path}");
            }
            T scene = _instantiate.GetOrAddComponent<T>(go);
            _uiSceneDict.Add(typeof(T), scene);
            go.transform.SetParent(Root.transform);
            return scene;
        }
        
        
        
        public T GetOrCreateSceneUI<T>(string name = null, string path = null) where T : UIScene
        {
            if (_uiSceneDict.TryGetValue(typeof(T), out UIScene value))
            {
                return value as T;
            }
            return GetSceneUIFromResource<T>(name, path);
        }



        public T MakeUIWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UIBase
        {
            if (name == null)
                name = typeof(T).Name;


            GameObject go = _instantiate.InstantiateByPath($"Prefabs/UI/WorldSpace/{name}");

            if (parent != null)
                go.transform.SetParent(parent);

            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.Log($"Failed to Load Canvas: GameObject Name:{go.name}");
                return null;
            }
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            return go.GetComponent<T>();
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null, string path = null) where T : UIBase
        {
            if (name == null)
                name = typeof(T).Name;

            GameObject go;
            if (path == null)
            {
                go = _instantiate.InstantiateByPath($"Prefabs/UI/SubItem/{name}");
            }
            else
            {
                go = _instantiate.InstantiateByPath($"{path}");
            }
            if (parent != null)
                go.transform.SetParent(parent);

            return _instantiate.GetOrAddComponent<T>(go);
        }

        public void ShowPopupUI(UIPopup popup)
        {
            IPopupHandler handler = popup as IPopupHandler;

            if (handler != null && handler.IsVisible == true)
                return;
            else if (handler == null && popup.gameObject.activeSelf == true)
                return;
            
            Canvas canvas = _instantiate.GetOrAddComponent<Canvas>(popup.gameObject);
            SetCanvas(canvas, true);
            _uiPopups.Push(popup);

            if (handler != null)
            {
                handler.ShowPopup();
            }
            else
            {
                popup.gameObject.SetActive(true);
            }
        
        }

        public void ClosePopupUI()
        {

            if (_uiPopups.Count <= 0)
                return;

            UIPopup popup = _uiPopups.Pop();

            IPopupHandler handler = popup as IPopupHandler;
            if (handler != null)
            {
                handler.ClosePopup();
            }
            else
            {
                popup.gameObject.SetActive(false);
            }
            _popupSorting--;
        }

        public void ClosePopupUI(UIPopup popup)
        {
            IPopupHandler handler = popup as IPopupHandler;

            if (handler != null && handler.IsVisible == false)
                return;
            else if (handler == null && popup.gameObject.activeSelf == false)
                return;


            Stack<UIPopup> tempUIPopupStack = new Stack<UIPopup>();

            while (_uiPopups.Count > 0)
            {
                UIPopup popupUI = _uiPopups.Pop();
                if (popupUI == popup)//나와 _ui_Popups에서 꺼낸 popup이 같다면 종료
                {
                    if (handler != null)
                    {
                        handler.ClosePopup();
                    }
                    else
                    {
                        popup.gameObject.SetActive(false);
                    }
                    _popupSorting--;
                    break;
                }
                else
                {
                    tempUIPopupStack.Push(popupUI); //아니라면 스택임시보관소에 저장
                }
            }

            while (tempUIPopupStack.Count > 0)//임시로 보관된 팝업창들을 다시 _ui_Popups에 붓는다.
            {
                _uiPopups.Push(tempUIPopupStack.Pop());
            }
        }

        public void CloseAllPopupUI()
        {
            while (_uiPopups.Count > 0)
                ClosePopupUI();
        }
        /// <summary>
        /// 팝업을 켜고 끌 수 있는 메서드 핸들러로 구현 하면 켜고 끄는 구현부를 직접수정할 수 있다.  
        /// </summary>
        /// <param name="popup">6.9일 IPopupHandler 인터페이스 추가 해당 인터페이스는 오브젝트를 켜고 끄는 방식의 핸들링을
        /// 수동으로 바꾸는 인터페이스이고 추가한 이유는 게임오브젝트를 비활성화하게 되면 백단에서 도는 프로세스가 멈추므로
        /// 백단에서 도는 프로세스가 있다면 해당 인터페이스를 상속받고 구현하는걸로 대체해야함</param>
        public void SwitchPopUpUI(UIPopup popup)
        {

            if (popup is IPopupHandler handler)
            {
                if (handler.IsVisible == false)
                {
                    ShowPopupUI(popup);
                }
                else
                {
                    ClosePopupUI(popup);
                }
            }
            else
            {
                if (popup.gameObject.activeSelf == false)
                {
                    ShowPopupUI(popup);
                }
                else
                {
                    ClosePopupUI(popup);
                }
            }
        }

        public bool GetTopPopUpUI(UIPopup popup)
        {
            if (_uiPopups.Count <= 0)
                return false;

            if (_uiPopups.Peek() == popup)
            {
                return true;
            }
            return false;
        }


        public void Clear()
        {
            CloseAllPopupUI();
            _uiSceneDict.Clear();
            _importantPopupUI.Clear();
            _uiPopups.Clear();
            _sorting = SceneUISortingDefaultValue;
            _popupSorting = PopupUISortingDefaultValue;
        }
    }
}
