using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class WaitTimeTarget : Conditional
    {
        private float _duration;
        private float _currentTime;
        [SerializeField]private SharedBool _hasArrived;
        [SerializeField]private SharedFloat _minSecond;
        [SerializeField]private SharedFloat _maxSecond;


        public override void OnStart()
        {
            base.OnStart();
            _duration = Random.Range(_minSecond.Value, _maxSecond.Value);
        }

        public override TaskStatus OnUpdate()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _duration)
            {
                return TaskStatus.Failure;
            }
            else
            {
                if (_hasArrived.Value)
                {
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Running;
        }


        public override void OnEnd()
        {
            base.OnEnd();
            _currentTime = 0;
            _duration = 0;
        }
    }
}
