using BehaviorDesigner.Runtime;
using Controller.BossState;
using Data.DataType.StatType;
using Util;

namespace Stats.BossStats.GolemStats
{
    public class BossGolemStats : global::Stats.BossStats.BossStats
    {
        private int _bossID;
        private BossGolemController _golemController;

        protected override void StartInit()
        {
            base.StartInit();
            _golemController = GetComponent<BossGolemController>();
            UpdateStat();
        }
        protected override void SetStats()
        {
            _bossID = (int)Define.BossID.Golem;
            BossStat stat = _statDict[_bossID];
            MaxHp = stat.hp;
            Hp = stat.hp;
            Attack = stat.attack;
            Defence = stat.defence;
            MoveSpeed = stat.speed;
            _viewAngle = stat.viewAngle;
            _viewDistance = stat.viewDistance;
        }

    

        protected override void OnDead(BaseStats.BaseStats attacker)
        {
            _golemController.CurrentStateType = _golemController.BaseDieState;
            _golemController.Anim.speed = 0.5f;
            GetComponent<BehaviorTree>().SendEvent("BossDeadEvent");
        }
    }
}