using GameManagers;
using GameManagers.Interface.GameManagerEx;
using Stats.BaseStats;
using UnityEngine;
using Util;
using Zenject;

namespace VFX
{
    public class ProjectorAttack : MonoBehaviour, IAttackRange
    {
        [Inject] IBossSpawnManager _bossSpawnManager;
        IIndicatorBahaviour _projector;
        private void Start()
        {
            _projector = GetComponent<IIndicatorBahaviour>();
        }

        public float ViewAngle => _projector.Angle;

        public float ViewDistance => _projector.Arc;
        public Transform OwnerTransform => _bossSpawnManager.GetBossMonster().transform;
        public Vector3 AttackPosition => transform.position;

        public LayerMask TarGetLayer { get => LayerMask.GetMask(
            Utill.GetLayerID(Define.ControllerLayer.Player),
            Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer)); }

    }
}
