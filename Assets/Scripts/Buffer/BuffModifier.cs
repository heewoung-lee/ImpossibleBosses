using Data.DataType.ItemType.Interface;
using Stats.BaseStats;

namespace Buffer
{
    public abstract class BuffModifier
    {
        public abstract string Buffname { get; }
        public abstract StatType StatType { get; }
        public abstract void ApplyStats(BaseStats stats, float value);

    }
}