using BehaviorDesigner.Runtime.Tasks;
using Stats.BaseStats;
using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class CheckBossHpBelowCondition : Conditional
    {
        [SerializeField]private int _hpPercent;
        private BaseStats _stat;
        public override void OnStart()
        {
            base.OnStart();
            _stat = Owner.GetComponent<BaseStats>();
        }
        public override TaskStatus OnUpdate()
        {
            if(_stat.Hp <= _stat.MaxHp/100*_hpPercent)
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
