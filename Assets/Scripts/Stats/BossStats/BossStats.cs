using System.Collections.Generic;
using UnityEngine;

public abstract class BossStats : BaseStats, IAttackRange
{
    protected float _viewAngle;
    protected float _viewDistance;
    protected Dictionary<int, BossStat> _statDict;
    private LayerMask _targetLayer;
    public float ViewAngle { get => _viewAngle; }
    public float ViewDistance { get => _viewDistance; }

    public Transform Owner_Transform => transform;

    public LayerMask TarGetLayer => _targetLayer;

    protected override void AwakeInit()
    {
        base.AwakeInit();
    }
    protected override void StartInit()
    {
        _targetLayer = LayerMask.GetMask("Player");
        _statDict = Managers.DataManager.AllDataDict[typeof(BossStat)] as Dictionary<int, BossStat>;
    }

}
