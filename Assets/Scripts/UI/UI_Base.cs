using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public abstract class UI_Base : NetworkBehaviour
{
    Dictionary<Type, Object[]> _bindDictionary = new Dictionary<Type,Object[]>();
    protected abstract void StartInit();
    protected abstract void AwakeInit();

    protected virtual void OnEnableInit(){}
    protected virtual void OnDisableInit(){}


    private void OnEnable()
    {
        OnEnableInit();
    }

    private void OnDisable()
    {
        OnDisableInit();
    }

    private void Awake()
    {
        AwakeInit();
    }

    private void Start()
    {
        StartInit();
    }
    protected void Bind<T>(Type type) where T : Object
    {

        if (type.IsEnum == false)
            return;

        string[] names = Enum.GetNames(type);
        Object[] objects = new Object[names.Length];
        objects = FindObjects<T>(objects, 0, names.Length, names);

        _bindDictionary.Add(typeof(T), objects);
    }

    protected void SetSortingOrder(int soringOrder)
    {
        gameObject.GetComponent<Canvas>().sortingOrder = soringOrder;
    }

    protected void AddBind<T>(Type type,out string[] indexString) where T: Object
    {
        
        if(_bindDictionary.ContainsKey(typeof(T)))
        {
            Object[] objects = _bindDictionary[typeof(T)];
            List<string> nameList = new List<string>();
            for(int beforeIndex=0; beforeIndex < objects.Length; beforeIndex++)
            {
                nameList.Add(objects[beforeIndex].name);
            }
            string[] names = Enum.GetNames(type);
            {
            for (int index=0; index < names.Length; index++)
                nameList.Add(names[index]);
            }
            Object[] newObjects = new Object[nameList.Count];
            Array.Copy(objects, newObjects, objects.Length);
            newObjects = FindObjects<T>(newObjects, objects.Length, newObjects.Length, nameList.ToArray());
            _bindDictionary[typeof(T)] = newObjects;
            indexString = nameList.ToArray();
        }
        else
        {
            Bind<T>(type);
            indexString = _bindDictionary[typeof(T)].Select(bindObject=>bindObject.name).ToArray();
        }
    }

    private Object[] FindObjects<T>(Object[] objects,int startIndex,int endIndex, string[] names) where T : Object
    {
        Object[] newObjects = objects;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                newObjects[i] = Utill.FindChild(gameObject, names[i], true);
            }
            else
            {
                newObjects[i] = Utill.FindChild<T>(gameObject, names[i], true);
            }
        }
        return newObjects;
    }



    protected T Get<T>(int idx) where T : Object
    {
        Object[] objects = null;

        if(_bindDictionary.TryGetValue(typeof(T),out objects) == false)
        {
            Debug.LogError($"not Found Object{typeof(T)}");
            return null;
        }
        return objects[idx] as T;
    }


    protected TMP_Text GetText(int idx) => Get<TMP_Text>(idx);

    protected Button GetButton(int idx) => Get<Button>(idx);

    protected Image GetImage(int idx) => Get<Image>(idx);

    protected GameObject GetObject(int idx) => Get<GameObject>(idx);

    // UI_Base�� ��� UI���� ��ӹ޴� �����ӿ�ũ
    //��ųʸ��� ���� UI_Base�� BindŸ�Ե��� Ű�� �����ϰ�, BINDŸ�Ծȿ� �ִ� �̸��� ���� ��ü���� �迭�� �����Ѵ�.

    //1) Bind�� ���� Enum Type�� ���ڷ� �ް� Enum type�ȿ� �ִ� ���ڵ��� �̸����� �޴´�.
    //�ش�Ÿ���� Gameobject��� �ش簴ü�� �����ϰ�, �ƴ϶��, ���׸� TŸ���� �����޴´�.
    //2) Get�� ���� ��ųʸ��� ����� Ÿ���� Enum�� �̸����� �����´�.
    //3) BindEvent�� ���� �ش簴ü�� �̺�Ʈ �ڵ鷯�� �޾��ش�.

    public static void BindEvent(GameObject go,Action<PointerEventData> action,Define.UI_Event mouseEvent = Define.UI_Event.LeftClick)
    {
        UI_EventHandler evt = go.GetOrAddComponent<UI_EventHandler>();

        switch (mouseEvent)
        {
            case Define.UI_Event.LeftClick:
                evt.onLeftClickEvent -= action;
                evt.onLeftClickEvent += action;
                break;
            case Define.UI_Event.RightClick:
                evt.onRightClickEvent -= action;
                evt.onRightClickEvent += action;
                break;
            case Define.UI_Event.Drag:
                evt.onDragEvent -= action;
                evt.onDragEvent += action;
                break;
            case Define.UI_Event.DragBegin:
                evt.onBeginDragEvent -= action;
                evt.onBeginDragEvent += action;
                break;
            case Define.UI_Event.DragEnd:
                evt.onEndDragEvent -= action;
                evt.onEndDragEvent += action;
                break;
            case Define.UI_Event.PointerEnter:
                evt.onPointerEnterEvent -= action;
                evt.onPointerEnterEvent += action;
                break;
            case Define.UI_Event.PointerExit:
                evt.onPointerExitEvent -= action;
                evt.onPointerExitEvent += action;
                break;
        }

    }

    protected Vector2 GetUISize(GameObject uiObject)
    {
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        RectTransform parentRect = rectTransform.parent as RectTransform;

        if (parentRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        }

        Vector2 size = rectTransform.rect.size;
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        return size;
    }

    protected Vector2 GetUIScreenPosition(RectTransform rectTr)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rectTr.position);
        return screenPos;
    }

}