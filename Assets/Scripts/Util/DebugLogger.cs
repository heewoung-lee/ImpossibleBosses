using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log($"{gameObject.name}�� Ȱ��ȭ�Ǿ����ϴ�.");
        Debug.Log($"Ȱ��ȭ ���� Ʈ���̽�:\n{System.Environment.StackTrace}");

    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject.name}�� ��Ȱ��ȭ�Ǿ����ϴ�.");
        Debug.Log($"��Ȱ��ȭ ���� Ʈ���̽�:\n{System.Environment.StackTrace}");
    }
}