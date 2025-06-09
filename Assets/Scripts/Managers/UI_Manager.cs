using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Manager : IManagerIResettable
{

    private const int SCENE_UI_SORTING_DEFAULT_VALUE = 0;
    private const int POPUP_UI_SORTING_DEFAULT_VALUE = 30;
    private const int DESCRIPTION_UI_SORTING_DEFAULT_VALUE = 50;


    int _sorting = SCENE_UI_SORTING_DEFAULT_VALUE;
    int _popupSorting = POPUP_UI_SORTING_DEFAULT_VALUE;
    int _descriptionUI_Sorting = DESCRIPTION_UI_SORTING_DEFAULT_VALUE;

    public int DescriptionUI_Sorting => _descriptionUI_Sorting;

    private Stack<UI_Popup> _ui_Popups = new Stack<UI_Popup>();

    private Dictionary<Type, UI_Scene> _ui_sceneDict = new Dictionary<Type, UI_Scene>();
    public Dictionary<Type, UI_Scene> UI_sceneDict => _ui_sceneDict;

    private Dictionary<Type, UI_Base> _importantPopup_UI = new Dictionary<Type, UI_Base>();

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

    public GameObject UI_DamageText_Root
    {
        get
        {
            GameObject go = GameObject.Find("@UI_DamageText");
            if (go == null)
            {
                go = new GameObject() { name = "@UI_DamageText" };
            }
            return go;
        }
    }

    public void AddImportant_Popup_UI(UI_Base important_UI)
    {
        Type type = important_UI.GetType();
        _importantPopup_UI[type] = important_UI;
    }

    public T GetImportant_Popup_UI<T>() where T : UI_Base
    {

        if (_importantPopup_UI.TryGetValue(typeof(T), out UI_Base value))
        {
            return value as T;
        }

        Debug.Log($"Not Found KeyType: {typeof(T)}");
        return null;
    }

    public T Get_Scene_UI<T>() where T : UI_Scene
    {

        if (_ui_sceneDict.TryGetValue(typeof(T), out UI_Scene value))
        {
            return value as T;
        }

        Debug.LogError($"Not Found KeyType: {typeof(T)}");
        return null;
    }

    public bool Try_Get_Scene_UI<T>(out T ui_scene) where T : UI_Scene
    {
        if (_ui_sceneDict.TryGetValue(typeof(T), out UI_Scene scene))
        {
            ui_scene = scene as T;
            return ui_scene is not null;
        }
        ui_scene = null;
        return false;
    }
    public void SetCanvas(Canvas canvas, bool sorting = false)//�� �Ѿ�� ���ʱ�ȭ �Ұ�
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        if (sorting)
        {
            if (canvas.GetComponent<UI_Popup>() != null)
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

    public T TryGetPopupInDict<T>() where T : UI_Popup
    {
        T popup = GetImportant_Popup_UI<T>();
        if (popup == null)
        {
            popup = GetPopupUIFromResource<T>();
            AddImportant_Popup_UI(popup);
            return popup;
        }
        return popup;
    }

    public T TryGetPopupDictAndShowPopup<T>() where T : UI_Popup
    {
        T ui_popup = TryGetPopupInDict<T>();
        Managers.UI_Manager.ShowPopupUI(ui_popup);
        return ui_popup;
    }

    public T GetPopupUIFromResource<T>(string name = null) where T : UI_Popup
    {
        if (name == null)
            name = typeof(T).Name;

        GameObject go = Managers.ResourceManager.Instantiate($"Prefabs/UI/Popup/{name}");
        T popup = Utill.GetOrAddComponent<T>(go);

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
    public T GetSceneUIFromResource<T>(string name = null, string path = null) where T : UI_Scene
    {
        if (name == null)
            name = typeof(T).Name;

        GameObject go = null;
        if (path == null)
        {
            go = Managers.ResourceManager.Instantiate($"Prefabs/UI/MainUI/{name}");
        }
        else
        {
            go = Managers.ResourceManager.Instantiate($"{path}");
        }
        T scene = Utill.GetOrAddComponent<T>(go);
        _ui_sceneDict.Add(typeof(T), scene);
        go.transform.SetParent(Root.transform);

        return scene;
    }
    public T GetOrCreateSceneUI<T>(string name = null, string path = null) where T : UI_Scene
    {
        if (_ui_sceneDict.TryGetValue(typeof(T), out UI_Scene value))
        {
            return value as T;
        }
        return GetSceneUIFromResource<T>(name, path);
    }



    public T MakeUIWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (name == null)
            name = typeof(T).Name;


        GameObject go = Managers.ResourceManager.Instantiate($"Prefabs/UI/WorldSpace/{name}");

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

    public T MakeSubItem<T>(Transform parent = null, string name = null, string path = null) where T : UI_Base
    {
        if (name == null)
            name = typeof(T).Name;

        GameObject go;
        if (path == null)
        {
            go = Managers.ResourceManager.Instantiate($"Prefabs/UI/SubItem/{name}");
        }
        else
        {
            go = Managers.ResourceManager.Instantiate($"{path}");
        }


        if (parent != null)
            go.transform.SetParent(parent);

        return Utill.GetOrAddComponent<T>(go);
    }

    public void ShowPopupUI(UI_Popup popup)
    {
        IPopupHandler handler = popup as IPopupHandler;

        if (handler != null && handler.IsVisible == true)
            return;
        else if (handler == null && popup.gameObject.activeSelf == true)
            return;

        Canvas canvas = Utill.GetOrAddComponent<Canvas>(popup.gameObject);
        SetCanvas(canvas, true);
        _ui_Popups.Push(popup);

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

        if (_ui_Popups.Count <= 0)
            return;

        UI_Popup popup = _ui_Popups.Pop();

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

    public void ClosePopupUI(UI_Popup popup)
    {
        IPopupHandler handler = popup as IPopupHandler;

        if (handler != null && handler.IsVisible == false)
            return;
        else if (handler == null && popup.gameObject.activeSelf == false)
            return;


        Stack<UI_Popup> tempUIPopupStack = new Stack<UI_Popup>();

        while (_ui_Popups.Count > 0)
        {
            UI_Popup popup_ui = _ui_Popups.Pop();
            if (popup_ui == popup)//���� _ui_Popups���� ���� popup�� ���ٸ� ����
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
                tempUIPopupStack.Push(popup_ui); //�ƴ϶�� �����ӽú����ҿ� ����
            }
        }

        while (tempUIPopupStack.Count > 0)//�ӽ÷� ������ �˾�â���� �ٽ� _ui_Popups�� �״´�.
        {
            _ui_Popups.Push(tempUIPopupStack.Pop());
        }
    }

    public void CloseAllPopupUI()
    {
        while (_ui_Popups.Count > 0)
            ClosePopupUI();
    }
    /// <summary>
    /// �˾��� �Ѱ� �� �� �ִ� �޼��� �ڵ鷯�� ���� �ϸ� �Ѱ� ���� �����θ� ���������� �� �ִ�.  
    /// </summary>
    /// <param name="popup">6.9�� IPopupHandler �������̽� �߰� �ش� �������̽��� ������Ʈ�� �Ѱ� ���� ����� �ڵ鸵��
    /// �������� �ٲٴ� �������̽��̰� �߰��� ������ ���ӿ�����Ʈ�� ��Ȱ��ȭ�ϰ� �Ǹ� ��ܿ��� ���� ���μ����� ���߹Ƿ�
    /// ��ܿ��� ���� ���μ����� �ִٸ� �ش� �������̽��� ��ӹް� �����ϴ°ɷ� ��ü�ؾ���</param>
    public void SwitchPopUpUI(UI_Popup popup)
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

    public bool GetTopPopUpUI(UI_Popup popup)
    {
        if (_ui_Popups.Count <= 0)
            return false;

        if (_ui_Popups.Peek() == popup)
        {
            return true;
        }
        return false;
    }


    public void Clear()
    {
        CloseAllPopupUI();
        _ui_sceneDict.Clear();
        _importantPopup_UI.Clear();
        _ui_Popups.Clear();
        _sorting = SCENE_UI_SORTING_DEFAULT_VALUE;
        _popupSorting = POPUP_UI_SORTING_DEFAULT_VALUE;
        _descriptionUI_Sorting = DESCRIPTION_UI_SORTING_DEFAULT_VALUE;
    }
}
