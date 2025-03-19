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

public static class Extension
{
    public static IItem SetIItemEffect(this IItem iteminfo,IteminfoStruct iteminfostruct)
    {
        IItem setIteminfo = iteminfo;

        if (setIteminfo.ItemEffects != null)
        {
            setIteminfo.ItemEffects.Clear(); // ���� ������ �ʱ�ȭ
            setIteminfo.ItemEffects.AddRange(iteminfostruct.ItemEffects); // ���ο� ������ �߰�
        }
        return iteminfo;
    }
}