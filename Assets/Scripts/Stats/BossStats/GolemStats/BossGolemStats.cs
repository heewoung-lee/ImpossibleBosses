using BaseStates;
using BehaviorDesigner.Runtime;

public class BossGolemStats : BossStats
{
    private int _bossID;
    private BossGolemController _golemController;

    protected override void StartInit()
    {
        base.StartInit();
        _golemController = GetComponent<BossGolemController>();
        UpdateStat();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
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

    

    protected override void OnDead(BaseStats attacker)
    {
        _golemController.CurrentStateType = _golemController.Base_DieState;
        _golemController.Anim.speed = 0.5f;
        GetComponent<BehaviorTree>().SendEvent("BossDeadEvent");
    }
}