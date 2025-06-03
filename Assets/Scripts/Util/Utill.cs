using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using static UnityEngine.InputSystem.PlayerInputManager;

public class Utill
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static Color GetItemGradeColor(Item_Grade_Type grade)
    {
        switch (grade)
        {
            case Item_Grade_Type.Normal:
                return Color.white;
            case Item_Grade_Type.Magic:
                return Color.green;
            case Item_Grade_Type.Rare:
                return new Color(150 / 255f, 200 / 255f, 255 / 255f);//�Ķ���;
            case Item_Grade_Type.Unique:
                return Color.red;
            case Item_Grade_Type.Epic:
                return Color.yellow;
        }
        return Color.white;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            //���� �ؿ� �ִ� �ڽ��� Ž���ؼ� component�� �����ָ� ��.
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(child.name) || child.name == name)
                {
                    T component = child.GetComponent<T>();
                    return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;
    }




    public static T[] FindChildAll<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        List<T> list = new List<T>();
        if (recursive == false)
        {
            //���� �ؿ� �ִ� �ڽ��� Ž���ؼ� component�� �����ָ� ��.
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(child.name) || child.name == name)
                {
                    list.Add(child.GetComponent<T>());
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    list.Add(component);
            }
        }

        if (list.Count > 0)
            return null;
        else
            return list.ToArray();
    }


    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);

        if (transform == null)
            return null;

        return transform.gameObject;
    }


    public static float GetAnimationLength(string animationName, Animator anim)
    {
        float time = 0;
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == animationName)
            {
                time = ac.animationClips[i].length;
            }
        }

        return time;
    }

    public static string ItemGradeConvertToKorean(Item_Grade_Type itemGrade)
    {
        switch (itemGrade)
        {
            case Item_Grade_Type.Normal:
                return "���";
            case Item_Grade_Type.Magic:
                return "����";
            case Item_Grade_Type.Rare:
                return "����";
            case Item_Grade_Type.Unique:
                return "����ũ";
            case Item_Grade_Type.Epic:
                return "����";
        }

        return "Unknown";
    }

    public static string StatTypeConvertToKorean(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHP:
                return "�ִ�ü��";
            case StatType.CurrentHp:
                return "ü��";
            case StatType.Attack:
                return "���ݷ�";
            case StatType.Defence:
                return "����";
            case StatType.MoveSpeed:
                return "�̵��ӵ�";
        }
        return "Unknown";
    }

    public static bool IsAlphanumeric(string input)
    {
        // ������ ���ڸ� ���Ե� ���ڿ����� Ȯ��
        return Regex.IsMatch(input, "^[A-Za-z0-9]+$");
    }

    //public static async Task<T> RateLimited<T>(Func<Task<T>> action, int millisecondsDelay = 1000)
    //{
    //    Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
    //    await Task.Delay(millisecondsDelay); // ���
    //    return await action.Invoke(); // ���޹��� �۾� ���� �� ��� ��ȯ
    //}
    //public static async Task RateLimited(Func<Task> action, int millisecondsDelay = 1000)
    //{
    //    Debug.LogWarning($"Rate limit exceeded. Retrying in {millisecondsDelay / 1000} seconds...");
    //    await Task.Delay(millisecondsDelay); // ���
    //    await action.Invoke(); // ���޹��� �۾� ���� �� ��� ��ȯ
    //}

    private static CancellationTokenSource _retryCts;
    private static CancellationTokenSource _retryCtsVoid;
    public static async Task<T> RateLimited<T>(Func<Task<T>> action, int delayMs = 1_000)
    {
        // 1) ���� �� CTS�� �����.
        var newCts = new CancellationTokenSource();

        // 2) ���� CTS�� ���������� ��ҡ�����ϰ�
        var prevCts = Interlocked.Exchange(ref _retryCts, newCts);
        prevCts?.Cancel();
        prevCts?.Dispose();

        try
        {
            Debug.LogWarning($"Rate limit exceeded. Retrying in {delayMs / 1000} seconds��");
            await Task.Delay(delayMs, newCts.Token);

            return await action();
        }
        catch (TaskCanceledException)
        {
            Debug.Log("RateLimited<T>: ���� ������ ��ҵǾ� �������� �ʽ��ϴ�.");
            return default;
        }
        finally
        {
            // ���� ���������� ����� CTS��� null �� �ʱ�ȭ
            Interlocked.CompareExchange(ref _retryCts, null, newCts);
            newCts.Dispose();
        }
    }

    public static async Task RateLimited(Func<Task> action,int delayMs = 1_000)
    {  // 1) ���� �� CTS�� �����.
        var newCts = new CancellationTokenSource();

        // 2) ���� CTS�� ���������� ��ҡ�����ϰ�
        var prevCts = Interlocked.Exchange(ref _retryCtsVoid, newCts);
        prevCts?.Cancel();
        prevCts?.Dispose();

        try
        {
            Debug.LogWarning($"Rate limit exceeded. Retrying in {delayMs / 1000} seconds��");
            await Task.Delay(delayMs, newCts.Token);
            await action();
        }
        catch (TaskCanceledException)
        {
            Debug.Log("RateLimited<T>: ���� ������ ��ҵǾ� �������� �ʽ��ϴ�.");
        }
        finally
        {
            // ���� ���������� ����� CTS��� null �� �ʱ�ȭ
            Interlocked.CompareExchange(ref _retryCtsVoid, null, newCts);
            newCts.Dispose();
        }
    }

    public static string GetLayerID(Enum enumvalue)
    {
        return enumvalue.ToString();
    }


  
}