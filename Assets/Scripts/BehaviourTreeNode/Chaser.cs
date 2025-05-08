using BaseStates;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("3278c95539f686f47a519013713b31ac", "9f01c6fc9429bae4bacb3d426405ffe4")]
    public class Chaser : NavMeshMovement
    {
        public GameObject targetOBject
        {

            set
            {
                if (_controller == null)
                {
                    _controller = Owner.GetComponent<BossGolemController>();
                }
                _controller.TargetObject = value;
            }
            get
            {
                if (_controller == null)
                {
                    _controller = Owner.GetComponent<BossGolemController>();
                }
                return _controller.TargetObject;
            }
        }
        [Tooltip("If target is null then use the target position")]
        [UnityEngine.Serialization.FormerlySerializedAs("targetPosition")]
        public SharedBool _hasArrived;
        private BossGolemController _controller;


        public override void OnAwake()
        {
            base.OnAwake();
            _controller = Owner.GetComponent<BossGolemController>();
        }
        public override void OnStart()
        {
            base.OnStart();
            _hasArrived.Value = false;

            if (targetOBject == null)
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
                    if (findClosePlayer == distance)
                    {
                        targetOBject = collider.transform.gameObject;
                    }
                }
            }
            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (targetOBject == null)
            {
                _controller.UpdateIdle();
                return TaskStatus.Failure;
            }

            _controller.UpdateMove();
            _hasArrived.Value = HasArrived() && TargetInSight.IsTargetInSight(_controller.GetComponent<IAttackRange>(), targetOBject.transform, 0.2f);

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
            if (targetOBject != null)
            {
                return targetOBject.transform.position;
            }
            return Vector3.zero;
        }

        public override void OnReset()
        {
            base.OnReset();
            targetOBject = null;
        }


        public override void OnEnd()
        {
            base.OnEnd();
            targetOBject = null;
        }

    }
}