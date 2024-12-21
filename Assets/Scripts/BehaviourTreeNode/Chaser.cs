using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
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
        [Tooltip("The GameObject that the agent is seeking")]
        [UnityEngine.Serialization.FormerlySerializedAs("target")]
        public SharedGameObject m_Target;
        [Tooltip("If target is null then use the target position")]
        [UnityEngine.Serialization.FormerlySerializedAs("targetPosition")]
        public SharedBool _hasArrived;
        private BossController<GolemAttackType> _controller;
        public override void OnStart()
        {
            base.OnStart();
            _hasArrived.Value = false;
            _controller = Owner.GetComponent<BossController<GolemAttackType>>();
            Collider[] checkedPlayer = Physics.OverlapSphere(transform.position, 10000f, LayerMask.GetMask("Player"));
            float findClosePlayer = float.MaxValue;
            foreach (Collider collider in checkedPlayer)
            {
                float distance = (transform.position - collider.transform.position).sqrMagnitude;
                findClosePlayer = findClosePlayer > distance ? distance : findClosePlayer;
                if (findClosePlayer == distance)
                    m_Target = collider.transform.gameObject;
            }
            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            _hasArrived.Value = HasArrived() && TargetInSight.IsTargetInSight(_controller.GetComponent<IAttackRange>(), m_Target.Value.transform, 0.2f);
            if (_hasArrived.Value)
            {
                SetDestination(transform.position);
                return TaskStatus.Success;
            }
            SetDestination(Target());
            _controller.SetStateMove();
            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (m_Target.Value != null)
            {
                return m_Target.Value.transform.position;
            }
            return Vector3.zero;
        }

        public override void OnReset()
        {
            base.OnReset();
            m_Target = null;
        }


        public override void OnEnd()
        {
            base.OnEnd();
            m_Target.Value = null;
        }
    }
}