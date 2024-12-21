using DTT.AreaOfEffectRegions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorAttack : MonoBehaviour, IAttackRange
{
    ArcRegionProjector projector;
    private void Start()
    {
        projector = GetComponent<ArcRegionProjector>();
    }

    public float ViewAngle => projector.Angle;

    public float ViewDistance => projector.Arc;

    public Transform Owner_Transform => gameObject.transform;

    public LayerMask TarGetLayer { get => LayerMask.GetMask("Player"); }
}
