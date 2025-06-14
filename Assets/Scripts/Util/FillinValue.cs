using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Util
{
    public class FillinValue
    {
        #region 필드자동으로 채워 넣기
#if UNITY_EDITOR
        /// <summary>
        /// 1.에셋에 있는 프리펩중 IPopulatingDefaultValue인터페이스를 구현한 클래스를 조사
        /// 2.인터페이스가 있는 오브젝트가 있다면 PopulatingDefaultValue()를 호출하고
        /// 3.에셋을 저장
        /// </summary>
        [MenuItem("PopulatingDefaultValue", menuItem = "Utility/PopulatingDefaultValue")]
        public static void PopulatingDefaultValue()
        {
            List<Object> allObjects = SearchAllObjects();
            foreach (Object obj in allObjects)
            {
                // IPopulatingDefaultValue 인터페이스를 구현했는지 확인
                if (obj is IPopulatingDefaultValue populatingDefaultValue)
                {
                    // PopulatingDefaultValue 메서드 호출
                    populatingDefaultValue.PopulatingDefaultValue(allObjects);
                    EditorUtility.SetDirty(obj);
                }
            }
            AssetDatabase.SaveAssets();  // 모든 변경된 에셋을 디스크에 저장합니다.
            AssetDatabase.Refresh();
        }

        private static List<Object> SearchAllObjects()
        {
            List<Object> searchedObjects = new List<Object>();
            string[] guids = AssetDatabase.FindAssets("t:Object", new[] { "Assets/FillableObjects" });  // 모든 프리펩 에셋을 검색
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
}
