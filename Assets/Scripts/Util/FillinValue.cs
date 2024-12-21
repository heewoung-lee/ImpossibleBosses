using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

public class FillinValue
{
    #region �ʵ��ڵ����� ä�� �ֱ�
#if UNITY_EDITOR
    /// <summary>
    /// 1.���¿� �ִ� �������� IPopulatingDefaultValue�������̽��� ������ Ŭ������ ����
    /// 2.�������̽��� �ִ� ������Ʈ�� �ִٸ� PopulatingDefaultValue()�� ȣ���ϰ�
    /// 3.������ ����
    /// </summary>
    [MenuItem("PopulatingDefaultValue", menuItem = "Utility/PopulatingDefaultValue")]
    public static void PopulatingDefaultValue()
    {
        List<Object> allObjects = SearchAllObjects();
        foreach (Object obj in allObjects)
        {
            // IPopulatingDefaultValue �������̽��� �����ߴ��� Ȯ��
            if (obj is IPopulatingDefaultValue populatingDefaultValue)
            {
                // PopulatingDefaultValue �޼��� ȣ��
                populatingDefaultValue.PopulatingDefaultValue(allObjects);
                EditorUtility.SetDirty(obj);
            }
        }
        AssetDatabase.SaveAssets();  // ��� ����� ������ ��ũ�� �����մϴ�.
        AssetDatabase.Refresh();
    }

    private static List<Object> SearchAllObjects()
    {
        List<Object> searchedObjects = new List<Object>();
        string[] guids = AssetDatabase.FindAssets("t:Object", new[] { "Assets/FillableObjects" });  // ��� ������ ������ �˻�
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            searchedObjects.AddRange(assets);
        }
        return searchedObjects;
    }
#endif
    #endregion
}
