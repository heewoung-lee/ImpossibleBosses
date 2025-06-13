using Stats.BaseStats;
using UnityEngine;

namespace Buffer
{
    public abstract class DurationBuff : BuffModifier
    {
        public abstract void RemoveStats(BaseStats stats, float value);
        public abstract Sprite BuffIconImage { get; }

        public abstract void SetBuffIconImage(Sprite buffImageIcon);
    }
}