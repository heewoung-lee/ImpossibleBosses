using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class CheckAttackProbability : Conditional
    {
        [SerializeField] private int _successRate = 0;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (Random.Range(0, 100) < _successRate)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

    }
}
