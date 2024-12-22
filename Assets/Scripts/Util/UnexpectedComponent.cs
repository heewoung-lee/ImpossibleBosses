using UnityEngine;

public class UnexpectedComponent : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log($"[DEBUG] {this.GetType().Name} added to {gameObject.name}\n{StackTraceUtility.ExtractStackTrace()}");
    }
}
