using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;


namespace BehaviourTreeNode.BossGolem.Task
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [BehaviorDesigner.Runtime.Tasks.HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("3278c95539f686f47a519013713b31ac", "9f01c6fc9429bae4bacb3d426405ffe4")]
    public class Chaser : NavMeshMovement
    {
        public GameObject TargetOBject
        {
            get
            {
                if (_controller == null)
                {
                    _controller = Owner.GetComponent<BossGolemController>();
                }
                return _controller.TargetObject;
            }
            set
            {
                if (_controller == null)
                {
                    _controller = Owner.GetComponent<BossGolemController>();
                }
                _controller.TargetObject = value;
            }
            
        }
        
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
        [UnityEngine.Serialization.FormerlySerializedAs("targetPosition")]
        public SharedBool _hasArrived;
        private BossGolemController _controller;
        private BossGolemAnimationNetworkController _bossGolemAnimationNetworkController;

        public override void OnAwake()
        {
            base.OnAwake();
            _controller = Owner.GetComponent<BossGolemController>();
            _bossGolemAnimationNetworkController = Owner.GetComponent<BossGolemAnimationNetworkController>();
        }
        public override void OnStart()
        {
            base.OnStart();
            _bossGolemAnimationNetworkController.SyncBossStateToClients(_controller.Base_MoveState);


            _hasArrived.Value = false;

            if (TargetOBject == null)
            {
                Collider[] checkedPlayer = Physics.OverlapSphere(transform.position, float.MaxValue, LayerMask.GetMask(
                    Utill.GetLayerID(Define.ControllerLayer.Player), Utill.GetLayerID(Define.ControllerLayer.AnotherPlayer)
                    ));
                float findClosePlayer = float.MaxValue;
                foreach (Collider collider in checkedPlayer)
                {

                    if (collider.TryGetComponent(out BaseStats baseStats))
                    {
                        if (baseStats.IsDead == true)
                            continue;
                    }

                    float distance = (transform.position - collider.transform.position).sqrMagnitude;
                    findClosePlayer = findClosePlayer > distance ? distance : findClosePlayer;
                    if (Mathf.Approximately(findClosePlayer, distance))
                    {
                        TargetOBject = collider.transform.gameObject;
                    }
                }
            }
            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (TargetOBject == null) // 타겟이 없는 경우 ex) 타겟이 다 죽은 경우
            {
                _controller.UpdateIdle();
                return TaskStatus.Failure;
            }

            _controller.UpdateMove();
            _hasArrived.Value = HasArrived() && TargetInSight.IsTargetInSight(_controller.GetComponent<IAttackRange>(), TargetOBject.transform, 0.2f);

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
            if (TargetOBject != null)
            {
                return TargetOBject.transform.position;
            }
            return Vector3.zero;
        }

        public override void OnReset()
        {
            base.OnReset();
            TargetOBject = null;
        }


        public override void OnEnd()
        {
            base.OnEnd();
            TargetOBject = null;
        }

    }
}