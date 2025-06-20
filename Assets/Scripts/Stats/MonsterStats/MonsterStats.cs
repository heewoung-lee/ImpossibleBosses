using System.Collections.Generic;
using Data.DataType.StatType;
using GameManagers;
using Zenject;

namespace Stats.MonsterStats
{
    public abstract class MonsterStats : BaseStats.BaseStats
    {
        protected Dictionary<int, MonsterStat> _statDict;
        [Inject] DataManager _dataManager;
        protected override void AwakeInit()
        {
            base.AwakeInit();
        }
        protected override void StartInit()
        {
            _statDict = _dataManager.AllDataDict[typeof(MonsterStat)] as Dictionary<int, MonsterStat>;
        }

    
    }
}
