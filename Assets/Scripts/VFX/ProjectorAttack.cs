using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorAttack : MonoBehaviour, IAttackRange
{
    IIndicatorBahaviour projector;
    private void Start()
    {
        projector = GetComponent<IIndicatorBahaviour>();
    }

    public float ViewAngle => projector.Angle;

    public float ViewDistance => projector.Arc;

    public Transform Owner_Transform => Managers.GameManagerEx.BossMonster.transform;
    //���⿡�� Owner_Transform�� ������ �ٲ���ϴµ� ��� �ٲ���ϳ�..
    public Vector3 AttackPosition => transform.position;

    public LayerMask TarGetLayer { get => LayerMask.GetMask(
        Utill.GetLayerID(Define.ControllerLayer.Player),
        Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer)); }

}
