using UnityEngine;

namespace Util
{
    /// <summary>
    /// 중심·반지름·세그먼트 수를 받아 Scene/Game 뷰에 원을 그려 줍니다.
    /// </summary>
    public static class DebugDrawUtill
    {
        /// <param name="center">원 중심</param>
        /// <param name="radius">반지름</param>
        /// <param name="segments">분할 수(선분 개수, 3 이상)</param>
        /// <param name="color">선 색상</param>
        /// <param name="duration">지속 시간; 0 = 한 프레임</param>
        public static void DrawCircle(Vector3 center,
            float radius,
            int segments = 32,
            Color? color = null,
            float duration = 0f)
        {
            if (segments < 3) segments = 3;
            Color lineColor = color ?? Color.white;

            float deltaAngle = 360f / segments;
            // 첫 점을 +Z 축 방향으로 잡음
            Vector3 prev = center + Vector3.forward * radius;

            for (int i = 1; i <= segments; i++)
            {
                // 다음 각도
                float angle = deltaAngle * i * Mathf.Deg2Rad;
                Vector3 next = center + new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;

                // prev → next 방향으로 레이(선분) 그리기
                Debug.DrawRay(prev, next - prev, lineColor, duration);

                prev = next;
            }
        }
    }
}
