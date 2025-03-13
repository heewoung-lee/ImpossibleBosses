using System.Collections.Generic;

public abstract class MonsterStats : BaseStats
{
    protected Dictionary<int, MonsterStat> _statDict;
    protected override void AwakeInit()
    {
        base.AwakeInit();
    }
    protected override void StartInit()
    {
        _statDict = Managers.DataManager.AllDataDict[typeof(MonsterStat)] as Dictionary<int, MonsterStat>;
    }

    
}
