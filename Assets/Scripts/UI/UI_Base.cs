using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    Dictionary<Type, UnityEngine.Object[]> _bindDictionary = new Dictionary<Type, UnityEngine.Object[]>();
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
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {

        if (type.IsEnum == false)
            return;

        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];


        for(int i = 0; i< names.Length; i++)
        {
            if(typeof(T) == typeof(GameObject))
            {
                objects[i] = Utill.FindChild(gameObject,names[i],true);
            }
            else
            {
                objects[i] = Utill.FindChild<T>(gameObject, names[i], true);
            }
        }

        _bindDictionary.Add(typeof(T), objects);
    }
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if(_bindDictionary.TryGetValue(typeof(T),out objects) == false)
            return null;

        return objects[idx] as T;
    }


    protected TMP_Text GetText(int idx) => Get<TMP_Text>(idx);

    protected Button GetButton(int idx) => Get<Button>(idx);

    protected Image GetImage(int idx) => Get<Image>(idx);

    protected GameObject GetObject(int idx) => Get<GameObject>(idx);



    // UI_Base는 모든 UI들이 상속받는 프레임워크
    //딕셔너리를 통해 UI_Base의 Bind타입들을 키로 저장하고, BIND타입안에 있는 이름과 같은 객체들을 배열로 저장한다.

    //1) Bind를 통해 Enum Type을 인자로 받고 Enum type안에 있는 인자들을 이름으로 받는다.
    //해당타입이 Gameobject라면 해당객체를 저장하고, 아니라면, 제네릭 T타입을 돌려받는다.
    //2) Get을 통해 딕셔너리에 저장된 타입을 Enum의 이름으로 꺼내온다.
    //3) BindEvent를 통해 해당객체에 이벤트 핸들러를 달아준다.

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

}