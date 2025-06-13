using Data.DataType.ItemType.Interface;
using Stats.BaseStats;
using UnityEngine;

namespace Buffer.SkillBuffer
{
    public class BufferDetermination : DurationBuff
    {
        public BufferDetermination(Sprite iconImage)
        {
            _iconImage = iconImage;
        }
        public override Sprite BuffIconImage => _iconImage;
        public override string Buffname => "방어력증가";
        public override StatType StatType => StatType.Defence;
        private Sprite _iconImage = null;

        public override void ApplyStats(BaseStats stats, float value)
        {
            stats.Plus_Defence_Abillity((int)value);
        }
        public override void RemoveStats(BaseStats stats, float value)
        {
            stats.Plus_Defence_Abillity(-(int)value);
        }

        public override void SetBuffIconImage(Sprite buffImageIcon)
        {
            _iconImage = buffImageIcon;
        }
    }
}