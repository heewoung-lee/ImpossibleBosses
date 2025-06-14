using System.Collections.Generic;
using Stats.BaseStats;
using UnityEngine;

namespace Util
{
    public class TargetInSight
    {
        public static int ID = 0;
        public static Vector3 BoundaryAngle(float angle, Transform playerTransform)
        {
            angle += playerTransform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public static void AttackTargetInSector(IAttackRange stats, int damage = -1)
        {
            Vector3 leftBoundary = BoundaryAngle(stats.ViewAngle * -0.5f, stats.OwnerTransform);
            Vector3 rightBoundary = BoundaryAngle(stats.ViewAngle * 0.5f, stats.OwnerTransform);

            Debug.DrawRay(stats.OwnerTransform.position + stats.OwnerTransform.up * 0.4f, leftBoundary * stats.ViewDistance, Color.red, 1f);
            Debug.DrawRay(stats.OwnerTransform.position + stats.OwnerTransform.up * 0.4f, rightBoundary * stats.ViewDistance, Color.red, 1f);

            Collider[] targets = Physics.OverlapSphere(stats.OwnerTransform.position, stats.ViewDistance, stats.TarGetLayer);
            foreach (Collider target in targets)
            {
                Transform targetTr = target.transform;
                if (target.TryGetComponent(out IDamageable idamaged))
                {
                    Vector3 direction = (targetTr.position - stats.OwnerTransform.position).normalized;
                    float angle = Vector3.Angle(direction, stats.OwnerTransform.forward);
                    if (angle < stats.ViewAngle * 0.5f)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(stats.OwnerTransform.position + stats.OwnerTransform.up * 0.4f, direction, out hit, stats.ViewDistance))
                        {
                            if (damage > 0)
                                idamaged.OnAttacked(stats, damage);
                            else
                                idamaged.OnAttacked(stats);

                        }
                    }
                }
            }
        }

        public static void AttackTargetInCircle(IAttackRange stats, float radius,int ?damage = null)
        {
            Collider[] targets = Physics.OverlapSphere(stats.AttackPosition, radius, stats.TarGetLayer);
            DebugDrawUtill.DrawCircle(stats.AttackPosition, radius, 96, Color.yellow, 5f);
            foreach (Collider target in targets)
            {
                if (target.TryGetComponent(out IDamageable idamaged))
                {
                    if(damage == null)
                        idamaged.OnAttacked(stats);
                    else
                        idamaged.OnAttacked(stats, damage.Value);
                }
            }
        }
        public static bool IsTargetInSight(IAttackRange stats, Transform targetTr, float sightRange = 0.5f)
        {
            Vector3 direction = (targetTr.position - stats.OwnerTransform.position).normalized;
            float angle = Vector3.Angle(direction, stats.OwnerTransform.forward);
            sightRange = Mathf.Clamp(sightRange, 0f, 0.5f);


            if (angle < stats.ViewAngle * sightRange)
            {
                return true;
            }

            return false;
        }

        public static List<Vector3> GeneratePositionsInSector(Transform originTr, float totalAngle, float totalRadius, int angleSteps, int radiusStep)
        {
            //현재 캐릭터의 좌표, 부채꼴의 각도, 반지름 길이, 각도에 따라 몇개로 나눌껀지, 길이에 따라 몇개로 나눌껀지
            //포지션을 담을 리스트, 단위당 길이, 단위당 각도

            List<Vector3> positions = new List<Vector3>();
            float anglePerUnit = totalAngle / (angleSteps - 1);
            float radiusPerUnit = totalRadius / radiusStep;
            float halfangle = totalAngle / 2f;

            for (int i = 1; i <= radiusStep; i++)
            {
                float currentRadius = radiusPerUnit * i;
                for (int j = 0; j < angleSteps; j++)
                {
                    //각도를 추출 
                    float currentAngle = -halfangle + anglePerUnit * j;//Degree -> Rad
                    float angleInRadian = (currentAngle + originTr.eulerAngles.y) * Mathf.Deg2Rad;
                    //삼각함수를 통해 단위 원에 대한 좌표를 출력
                    Vector3 perUnitPos = new Vector3(Mathf.Sin(angleInRadian), 0f, Mathf.Cos(angleInRadian));
                    //내위치와 길이단위에 맞게 조정
                    Vector3 currentPos = originTr.position + perUnitPos * currentRadius;
                    positions.Add(currentPos);
                }
            }

            return positions;
        }

        public static List<Vector3> GeneratePositionsInCircle(Transform originTr, float totalRadius, int angleSteps, int radiusStep)
        {
            return GeneratePositionsInSector(originTr, 360, totalRadius, angleSteps, radiusStep);

        }

    }
}