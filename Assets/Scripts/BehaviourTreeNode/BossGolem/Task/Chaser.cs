using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Controller.BossState;
using NetWork.Boss_NGO;
using Stats.BaseStats;
using UnityEngine;
using Util;


namespace BehaviourTreeNode.BossGolem.Task
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [BehaviorDesigner.Runtime.Tasks.HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("3278c95539f686f47a519013713b31ac", "9f01c6fc9429bae4bacb3d426405ffe4")]
    public class Chaser : NavMeshMovement
    {
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
        [UnityEngine.Serialization.FormerlySerializedAs("targetPosition")]
        [SerializeField]private SharedBool _hasArrived;
        private BossGolemController _controller;
        private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;
        private GameObject _targetObject;
        private Collider[] _targetObjects;

        public override void OnAwake()
        {
            base.OnAwake();
            _controller = Owner.GetComponent<BossGolemController>();
            _bossGolemAnimationNetworkController = Owner.GetComponent<BossGolemAnimationNetworkController>();
            _targetObjects = new Collider[Define.MaxPlayer];
        }
        public override void OnStart()
        {
            base.OnStart();
            _bossGolemAnimationNetworkController.SyncBossStateToClients(_controller.BaseMoveState);
            _hasArrived.Value = false;

            if (_targetObject == null)
            {
                Physics.OverlapSphereNonAlloc(transform.position, float.MaxValue, _targetObjects, 
                    LayerMask.GetMask(Utill.GetLayerID(Define.ControllerLayer.Player), Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer)
                ));
                float findClosePlayer = float.MaxValue;
                foreach (Collider collider in _targetObjects)
                {
                    if(collider == null)
                        continue;
                    
                    if (collider.TryGetComponent(out BaseStats baseStats))
                    {
                        if (baseStats.IsDead == true)
                            continue;
                    }

                    float distance = (transform.position - collider.transform.position).sqrMagnitude;
                    findClosePlayer = findClosePlayer > distance ? distance : findClosePlayer;
                    if (Mathf.Approximately(findClosePlayer, distance))
                    {
                        _targetObject = collider.transform.gameObject;
                    }
                }
            }
            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (_targetObject == null) // 타겟이 없는 경우 ex) 타겟이 다 죽은 경우
            {
                _controller.UpdateIdle();
                return TaskStatus.Failure;
            }

            _controller.UpdateMove();
            _hasArrived.Value = HasArrived() && TargetInSight.IsTargetInSight(_controller.GetComponent<IAttackRange>(), _targetObject.transform, 0.2f);

            if (_hasArrived.Value)
            {
                SetDestination(transform.position);
                return TaskStatus.Success;
            }
            SetDestination(Target());
            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (_targetObject != null)
            {
                return _targetObject.transform.position;
            }
            return Vector3.zero;
        }

        public override void OnReset()
        {
            base.OnReset();
            _targetObject = null;
        }


        public override void OnEnd()
        {
            base.OnEnd();
            _targetObject = null;
        }

    }
}