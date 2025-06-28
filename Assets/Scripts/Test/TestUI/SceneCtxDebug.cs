using UnityEngine;

namespace Test.TestUI
{
    public class SceneCtxDebug : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log($"[SceneCtxDebug] Awake  →  {gameObject.scene.name}");
        }

        void OnDestroy()
        {
            Debug.Log($"[SceneCtxDebug] Destroy →  {gameObject.scene.name}");
        }
    }
}