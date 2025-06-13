using System.Collections.Generic;
using Data.DataType.StatType;
using GameManagers;

namespace Stats.MonsterStats
{
    public abstract class MonsterStats : BaseStats.BaseStats
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
}
