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
using Unity.Netcode;
using System.Threading.Tasks;

public static class Extension
{
    public static IItem SetIItemEffect(this IItem iteminfo, IteminfoStruct iteminfostruct)
    {
        IItem setIteminfo = iteminfo;

        if (setIteminfo.ItemEffects != null)
        {
            setIteminfo.ItemEffects.Clear(); // 기존 데이터 초기화
            setIteminfo.ItemEffects.AddRange(iteminfostruct.ItemEffects); // 새로운 데이터 추가
        }
        return iteminfo;
    }
    public static bool TryGetComponentInParents<T>(this Transform searchPosition,out T findObject) where T : Component
    {
        Transform tr = searchPosition;

        while(tr != null)
        {
            if (tr.TryGetComponent(out T component))
            {
                findObject = component;
                return true;
            }
            else
            {
                tr = tr.parent;
            }
        }
        Debug.Log("Can't Find Object");
        findObject = null;
        return false;
    }


    public static GameObject RemoveCloneText(this GameObject go)
    {
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }


    public static async Task SafeFireAndForgetInvoke<T>(this Func<T,Task> asyncAction,T arg)
    {
        if (asyncAction == null) return;

        try
        {
            await asyncAction.Invoke(arg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);   // 예외 즉시 기록
        }
    }
}