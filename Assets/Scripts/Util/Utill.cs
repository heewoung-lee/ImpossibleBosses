using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                return new Color(150 / 255f, 200 / 255f, 255 / 255f);//파란색;
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
            //제일 밑에 있는 자식을 탐색해서 component를 돌려주면 됨.
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
            //제일 밑에 있는 자식을 탐색해서 component를 돌려주면 됨.
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
                return "노멀";
            case Item_Grade_Type.Magic:
                return "매직";
            case Item_Grade_Type.Rare:
                return "레어";
            case Item_Grade_Type.Unique:
                return "유니크";
            case Item_Grade_Type.Epic:
                return "에픽";
        }

        return "Unknown";
    }

    public static string StatTypeConvertToKorean(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHP:
                return "최대체력";
            case StatType.CurrentHp:
                return "체력";
            case StatType.Attack:
                return "공격력";
            case StatType.Defence:
                return "방어력";
            case StatType.MoveSpeed:
                return "이동속도";
        }
        return "Unknown";
    }
}