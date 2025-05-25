using UnityEngine;

/// <summary>
/// �߽ɡ������������׸�Ʈ ���� �޾� Scene/Game �信 ���� �׷� �ݴϴ�.
/// </summary>
public static class DebugDrawUtill
{
    /// <param name="center">�� �߽�</param>
    /// <param name="radius">������</param>
    /// <param name="segments">���� ��(���� ����, 3 �̻�)</param>
    /// <param name="color">�� ����</param>
    /// <param name="duration">���� �ð�; 0 = �� ������</param>
    public static void DrawCircle(Vector3 center,
                                  float radius,
                                  int segments = 32,
                                  Color? color = null,
                                  float duration = 0f)
    {
        if (segments < 3) segments = 3;
        Color lineColor = color ?? Color.white;

        float deltaAngle = 360f / segments;
        // ù ���� +Z �� �������� ����
        Vector3 prev = center + Vector3.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            // ���� ����
            float angle = deltaAngle * i * Mathf.Deg2Rad;
            Vector3 next = center + new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;

            // prev �� next �������� ����(����) �׸���
            Debug.DrawRay(prev, next - prev, lineColor, duration);

            prev = next;
        }
    }
}
