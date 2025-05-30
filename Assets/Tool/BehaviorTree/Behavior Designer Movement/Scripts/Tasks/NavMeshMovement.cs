﻿using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public abstract class NavMeshMovement : Movement
    {
        [Tooltip("The speed of the agent")]
        [UnityEngine.Serialization.FormerlySerializedAs("speed")]
        public SharedFloat m_Speed = 10;
        [Tooltip("The angular speed of the agent")]
        [UnityEngine.Serialization.FormerlySerializedAs("angularSpeed")]
        public SharedFloat m_AngularSpeed = 120;
        [Tooltip("The agent has arrived when the destination is less than the specified amount. This distance should be greater than or equal to the NavMeshAgent StoppingDistance.")]
        [UnityEngine.Serialization.FormerlySerializedAs("arriveDistance")]
        public SharedFloat m_ArriveDistance = 0.2f;
        [Tooltip("Should the NavMeshAgent be stopped when the task ends?")]
        [UnityEngine.Serialization.FormerlySerializedAs("stopOnTaskEnd")]
        public SharedBool m_StopOnTaskEnd = true;
        [Tooltip("Should the NavMeshAgent rotation be updated when the task ends?")]
        [UnityEngine.Serialization.FormerlySerializedAs("updateRotation")]
        public SharedBool m_UpdateRotation = true;

        // Component references
        protected NavMeshAgent m_NavMeshAgent;
        private bool m_StartUpdateRotation;

        /// <summary>
        /// Cache the component references.
        /// </summary>
        public override void OnAwake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        /// <summary>
        /// Allow pathfinding to resume.
        /// </summary>
        public override void OnStart()
        {
            m_NavMeshAgent.speed = m_Speed.Value;
            m_NavMeshAgent.angularSpeed = m_AngularSpeed.Value;
            m_NavMeshAgent.isStopped = false;
            m_StartUpdateRotation = m_NavMeshAgent.updateRotation;
            UpdateRotation(m_UpdateRotation.Value);
        }

        /// <summary>
        /// Set a new pathfinding destination.
        /// </summary>
        /// <param name="destination">The destination to set.</param>
        /// <returns>True if the destination is valid.</returns>
        protected override bool SetDestination(Vector3 destination)
        {
            m_NavMeshAgent.isStopped = false;
            return m_NavMeshAgent.SetDestination(destination);
        }

        /// <summary>
        /// Specifies if the rotation should be updated.
        /// </summary>
        /// <param name="update">Should the rotation be updated?</param>
        protected override void UpdateRotation(bool update)
        {
            m_NavMeshAgent.updateRotation = update;
            m_NavMeshAgent.updateUpAxis = update;
        }

        /// <summary>
        /// Does the agent have a pathfinding path?
        /// </summary>
        /// <returns>True if the agent has a pathfinding path.</returns>
        protected override bool HasPath()
        {
            return m_NavMeshAgent.hasPath && m_NavMeshAgent.remainingDistance > m_ArriveDistance.Value;
        }

        /// <summary>
        /// Returns the velocity of the agent.
        /// </summary>
        /// <returns>The velocity of the agent.</returns>
        protected override Vector3 Velocity()
        {
            return m_NavMeshAgent.velocity;
        }

        /// <summary>
        /// Returns true if the position is a valid pathfinding position.
        /// </summary>
        /// <param name="position">The position to sample. The position will be updated to the valid sampled position.</param>
        /// <returns>True if the position is a valid pathfinding position.</returns>
        protected bool SamplePosition(ref Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, m_NavMeshAgent.height * 2, NavMesh.AllAreas)) {
                position = hit.position;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Has the agent arrived at the destination?
        /// </summary>
        /// <returns>True if the agent has arrived at the destination.</returns>
        protected override bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (m_NavMeshAgent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = m_NavMeshAgent.remainingDistance;
            }

            return remainingDistance <= m_ArriveDistance.Value;
        }

        /// <summary>
        /// Stop pathfinding.
        /// </summary>
        protected override void Stop()
        {
            UpdateRotation(m_StartUpdateRotation);
            if (m_NavMeshAgent.hasPath) {
                m_NavMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// The task has ended. Stop moving.
        /// </summary>
        public override void OnEnd()
        {
            if (m_StopOnTaskEnd.Value) {
                Stop();
            } else {
                UpdateRotation(m_StartUpdateRotation);
            }
        }

        /// <summary>
        /// The behavior tree has ended. Stop moving.
        /// </summary>
        public override void OnBehaviorComplete()
        {
            Stop();
        }

        /// <summary>
        /// Reset the values back to their defaults.
        /// </summary>
        public override void OnReset()
        {
            m_Speed = 10;
            m_AngularSpeed = 120;
            m_ArriveDistance = 1;
            m_StopOnTaskEnd = true;
        }
    }
}