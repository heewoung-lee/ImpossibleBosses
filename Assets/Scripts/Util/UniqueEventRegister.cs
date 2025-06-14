using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Util
{
    public static class UniqueEventRegister
    {
        public static void AddSingleEvent<T>(ref T eventSource,T toaddEvent, [CallerMemberName] string callerName = "") where T : Delegate
        {
            //Action이 널이 아니고 이미 action에 들어가 있는 델리게이트 라면 반환
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
}