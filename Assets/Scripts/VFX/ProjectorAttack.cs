using System.Collections;
using System.Collections.Generic;
using GameManagers;
using Stats.BaseStats;
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

    public Transform OwnerTransform => Managers.GameManagerEx.BossMonster.transform;
    //여기에서 Owner_Transform을 보스로 바꿔야하는데 어떻게 바꿔야하나..
    public Vector3 AttackPosition => transform.position;

    public LayerMask TarGetLayer { get => LayerMask.GetMask(
        Utill.GetLayerID(Define.ControllerLayer.Player),
        Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer)); }

}
