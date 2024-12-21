using UnityEditor;
using UnityEngine;

public class SelectGameObjectsWithMissingScripts : Editor
{
    [MenuItem("Utility/Remove Missing Script")]
    private static void RemoveAllMissingScriptComponents()
    {
        GameObject selectedGameObjects = Selection.activeGameObject;
        int totalComponentCount = 0;
        int totalGameObjectCount = 0;




        foreach (Transform transform in selectedGameObjects.GetComponentsInChildren<Transform>(true))
        {
            int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);

            if (missingScriptCount > 0)
            {
                Undo.RegisterCompleteObjectUndo(transform.gameObject, "Remove Missing Scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transform.gameObject);

                totalComponentCount += missingScriptCount;
                totalGameObjectCount++;
            }

        }
        
        Debug.Log($"Removed {totalComponentCount} missing script component(s) from {totalGameObjectCount} game object(s).");
    }
}