using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UniqueEventRegister
{
    public static void AddSingleEvent<T>(ref T eventSource,T toaddEvent, [CallerMemberName] string callerName = "") where T : Delegate
    {
        //Action�� ���� �ƴϰ� �̹� action�� �� �ִ� ��������Ʈ ��� ��ȯ
        if (eventSource != null && eventSource.GetInvocationList().Contains(toaddEvent) == true)
        {
            Debug.Log($"{callerName} is already registered");
            return;
        }

        eventSource = (T)Delegate.Combine(eventSource, toaddEvent);
    }

    public static void RemovedEvent<T>(ref T eventSource,T removeEvent, [CallerMemberName] string callerName = "") where T : Delegate
    {
        if(eventSource == null || eventSource.GetInvocationList().Contains(removeEvent) == false)
        {
            Debug.Log($"{callerName} is not registered");
            return;
        }

        eventSource = (T)Delegate.Remove(eventSource, removeEvent);
    
    }

}