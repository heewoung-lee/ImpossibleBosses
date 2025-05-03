using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorAttack : MonoBehaviour, IAttackRange
{
    Indicator_Controller projector;
    private void Start()
    {
        projector = GetComponent<Indicator_Controller>();
    }

    public float ViewAngle => projector.Angle;

    public float ViewDistance => projector.Arc;

    public Transform Owner_Transform => gameObject.transform;

    public LayerMask TarGetLayer { get => LayerMask.GetMask(
        Utill.GetLayerID(Define.ControllerLayer.Player),
        Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer)); }
}
