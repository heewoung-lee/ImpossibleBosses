using System.Collections.Generic;
using Data.DataType.StatType;
using GameManagers;
using GameManagers.Interface.DataManager;
using Zenject;

namespace Stats.MonsterStats
{
    public abstract class MonsterStats : BaseStats.BaseStats
    {
        protected Dictionary<int, MonsterStat> _statDict;
        [Inject] IAllData _allData;
        protected override void AwakeInit()
        {
            base.AwakeInit();
        }
        protected override void StartInit()
        {
            _statDict = _allData.GetData(typeof(MonsterStat)) as Dictionary<int, MonsterStat>;
        }

    
    }
}
