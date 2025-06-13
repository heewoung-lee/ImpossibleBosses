using Data.DataType.ItemType.Interface;
using GameManagers;
using Stats.BaseStats;
using UnityEngine;

namespace Buffer.Buffer_Type.DurationBuffer
{
    public class BufferDefenceModifier : DurationBuff
    {
        public override string Buffname => "방어력증가";

        public override StatType StatType => StatType.Defence;

        private Sprite _iconImage = Managers.ResourceManager.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Shield");

        public override Sprite BuffIconImage => _iconImage;

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