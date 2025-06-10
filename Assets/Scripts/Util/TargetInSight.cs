using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem.XR;
using System;
using Unity.Netcode;
using static Unity.Cinemachine.CinemachineTargetGroup;

public class TargetInSight
{
    public static int ID = 0;
    public static Vector3 BoundaryAngle(float _angle, Transform playerTransform)
    {
        _angle += playerTransform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    public static void AttackTargetInSector(IAttackRange _stats, int damage = -1)
    {
        Vector3 _leftBoundary = BoundaryAngle(_stats.ViewAngle * -0.5f, _stats.Owner_Transform);
        Vector3 _rightBoundary = BoundaryAngle(_stats.ViewAngle * 0.5f, _stats.Owner_Transform);

        Debug.DrawRay(_stats.Owner_Transform.position + _stats.Owner_Transform.up * 0.4f, _leftBoundary * _stats.ViewDistance, Color.red, 1f);
        Debug.DrawRay(_stats.Owner_Transform.position + _stats.Owner_Transform.up * 0.4f, _rightBoundary * _stats.ViewDistance, Color.red, 1f);

        Collider[] _targets = Physics.OverlapSphere(_stats.Owner_Transform.position, _stats.ViewDistance, _stats.TarGetLayer);
        foreach (Collider _target in _targets)
        {
            Transform _targetTr = _target.transform;
            if (_target.TryGetComponent(out IDamageable idamaged))
            {
                Vector3 _direction = (_targetTr.position - _stats.Owner_Transform.position).normalized;
                float _angle = Vector3.Angle(_direction, _stats.Owner_Transform.forward);
                if (_angle < _stats.ViewAngle * 0.5f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(_stats.Owner_Transform.position + _stats.Owner_Transform.up * 0.4f, _direction, out hit, _stats.ViewDistance))
                    {
                        if (damage > 0)
                            idamaged.OnAttacked(_stats, damage);
                        else
                            idamaged.OnAttacked(_stats);

                    }
                }
            }
        }
    }

    public static void AttackTargetInCircle(IAttackRange _stats, float radius,int ?damage = null)
    {
        Collider[] _targets = Physics.OverlapSphere(_stats.AttackPosition, radius, _stats.TarGetLayer);
        DebugDrawUtill.DrawCircle(_stats.AttackPosition, radius, 96, Color.yellow, 5f);
        foreach (Collider _target in _targets)
        {
            Transform _targetTr = _target.transform;
            if (_target.TryGetComponent(out IDamageable idamaged))
            {
                if(damage == null)
                    idamaged.OnAttacked(_stats);
                else
                    idamaged.OnAttacked(_stats, damage.Value);
            }
        }
    }
    public static bool IsTargetInSight(IAttackRange _stats, Transform targetTr, float sightRange = 0.5f)
    {
        Vector3 direction = (targetTr.position - _stats.Owner_Transform.position).normalized;
        float angle = Vector3.Angle(direction, _stats.Owner_Transform.forward);
        sightRange = Mathf.Clamp(sightRange, 0f, 0.5f);


        if (angle < _stats.ViewAngle * sightRange)
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