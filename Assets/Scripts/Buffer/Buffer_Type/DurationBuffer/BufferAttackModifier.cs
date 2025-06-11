using Data.DataType.ItemType.Interface;
using GameManagers;
using UnityEngine;

namespace Buffer.Buffer_Type.DurationBuffer
{
    public class BufferAttackModifier : DurationBuff
    {
        public override Sprite BuffIconImage => _iconImage;
        public override string Buffname => "공격력증가";
        public override StatType StatType => StatType.Attack;

        private Sprite _iconImage = Managers.ResourceManager.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Sword");

        public override void ApplyStats(BaseStats stats, float value)
        {
            stats.Plus_Attack_Ability((int)value);
        }
        public override void RemoveStats(BaseStats stats, float value)
        {
            stats.Plus_Attack_Ability(-(int)value);
        }
        public override void SetBuffIconImage(Sprite buffImageIcon)
        {
            _iconImage = buffImageIcon;
        }
    }
}