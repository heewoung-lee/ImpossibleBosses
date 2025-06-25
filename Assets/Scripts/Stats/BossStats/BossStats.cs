using System.Collections.Generic;
using Data.DataType.StatType;
using GameManagers;
using GameManagers.Interface.DataManager;
using Stats.BaseStats;
using UnityEngine;
using Util;
using Zenject;

namespace Stats.BossStats
{
    public abstract class BossStats : BaseStats.BaseStats, IAttackRange
    {
        protected float _viewAngle;
        protected float _viewDistance;
        protected Dictionary<int, BossStat> _statDict;
        private LayerMask _targetLayer;
        
        [Inject] IAllData _allData;
        
        public float ViewAngle { get => _viewAngle; }
        public float ViewDistance { get => _viewDistance; }

        public Transform OwnerTransform => transform;

        public LayerMask TarGetLayer => _targetLayer;

        public Vector3 AttackPosition => transform.position;
        protected override void AwakeInit()
        {
            base.AwakeInit();
        }
        protected override void StartInit()
        {
            _targetLayer = LayerMask.GetMask(Utill.GetLayerID(Define.ControllerLayer.Player), Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer));
            _statDict = _allData.GetData(typeof(BossStat)) as Dictionary<int, BossStat>;
        }

    }
}
