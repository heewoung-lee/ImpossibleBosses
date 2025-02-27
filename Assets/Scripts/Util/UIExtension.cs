using System;
using UnityEngine.EventSystems;
using UnityEngine;
using BehaviorDesigner.Runtime;
using Unity.VisualScripting;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Events;

public static class UIExtension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Utill.GetOrAddComponent<T>(go);
    }

    public static void AddUIEvent(this GameObject go, Action<PointerEventData> action, Define.UI_Event mouseEvent = Define.UI_Event.LeftClick)
    {
        UI_Base.BindEvent(go, action, mouseEvent);
    }

    public static bool TryGetTask<T>(this BehaviorTree tree, out T task) where T : BehaviorDesigner.Runtime.Tasks.Task
    {
        task = tree.FindTask<T>();

        return task != null;
    }

    public static bool TryGetObject<T>(this System.Object originObject, out T originValue) where T : UnityEngine.Object
    {
        originValue = originObject as T;

        return originValue != null;
    }
    public static Color SetGradeColor(this MaskableGraphic graphic, Color setColor)
    {
        if (graphic != null)
        {
            graphic.color = setColor; // MaskableGraphic의 color 속성 설정
            return graphic.color; // 설정된 색상을 반환
        }
        return Color.white; // 그래픽이 null이면 기본 색상 반환
    }
    public static bool TryGetComponentInsChildren<T>(this GameObject origin, out T[] getComponent) where T : UnityEngine.Object
    {
        getComponent = origin.GetComponentsInChildren<T>();
        if (getComponent == null)
            return false;
        else
            return true;
    }
    public static bool TryGetComponentInChildren<T>(this GameObject origin, out T getComponent) where T : UnityEngine.Object
    {
        getComponent = origin.GetComponentInChildren<T>();
        if (getComponent == null)
            return false;
        else
            return true;
    }


    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        return Utill.FindChild<T>(go, name, recursive);
    }


    public static T FindParantComponent<T>(this Transform currentTr)
    {
        Transform parent = currentTr.parent;
        while (parent != null)
        {
            T component = parent.GetComponent<T>();
            if (component == null)
            {
                parent = parent.parent;
            }
            else
            {
                return component;
            }
        }
        return default(T);
    }
    public static bool TryGetFindParentObject<T>(this Transform currentTr, out T findObject) where T : class
    {
        Transform parent = currentTr.parent;
        while (parent != null)
        {
            foreach (Component obj in parent.GetComponents<Component>())
            {
                if (obj is T)
                {
                    findObject = obj as T;
                    return true;
                }
            }
            parent = parent.parent;
        }
        findObject = null;
        return false;
    }


    public static UI_ItemComponent_Inventory MakeInventoryItemComponent(this IItem iteminfo, Transform parentTr = null, int itemCount = 1, string name = null, string path = null)
    {
        //iteminfo의 정보는 ItemConsumable 혹은 ItemEquipment이기에 UI_ItemComponent 형변환이 안된다.
        //ItemCOnsumable을 찾으면 UI_ItemComponent_Consumable로 연결해주고
        //ItemEquipment을 찾으면 UI_ItemComponent_Equipment로 연결해야한다.

        UI_Player_Inventory inventory = Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>();

        if (parentTr == null)
        {
            parentTr = Managers.LootItemManager.GetItemComponentPosition(inventory);
        }

        IInventoryItemMaker itemUIType = iteminfo as IInventoryItemMaker;
         return itemUIType.MakeItemComponentInventory(parentTr, itemCount, name, path);
    }

    public static UI_ShopItemComponent MakeShopItemComponent(this IItem iteminfo,int price,Transform parentTr = null, int itemCount = 1, string name = null, string path = null)
    {
        UI_Shop shop = Managers.UI_Manager.GetImportant_Popup_UI<UI_Shop>();
        if (parentTr == null)
            parentTr = shop.ItemCoordinate;

        IShopItemMaker itemUIType = iteminfo as IShopItemMaker;
        return itemUIType.MakeShopItemComponent(price,parentTr, itemCount, name, path);
    }

    public static Color HexCodetoConvertColor(this string hexCode)
    {
        if(UnityEngine.ColorUtility.TryParseHtmlString(hexCode,out Color color))
        {
            return color;
        }
        return Color.white;   
    }

    public static UI_AlertPopupBase AfterAlertEvent(this UI_AlertPopupBase dialog,UnityAction action)
    {
        dialog.SetCloseButtonOverride(action);
        return dialog;
    }

    public static UI_AlertPopupBase AlertSetText(this UI_AlertPopupBase dialog, string titleText,string bodyText)
    {
        dialog.SetText(titleText, bodyText);
        return dialog;
    }


    public static Image ImageEnable(this Image image,bool isLobbyLoading)
    {
        image.enabled = isLobbyLoading;
        Debug.Log($"ImageEnable 호출완료{isLobbyLoading}");
        return image;
    }
}