using Data.DataType.ItemType.Interface;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Stats.BaseStats;
using UnityEngine;
using Zenject;

namespace Buffer.Buffer_Type.DurationBuffer
{
    public class BufferAttackModifier : DurationBuff
    {
        
        [Inject]private IResourcesLoader _loader;

        private Sprite _iconImage;

        public override Sprite BuffIconImage
        {
            get
            {
                if (_iconImage == null)
                {
                    _iconImage = _loader.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Sword");
                }
                return _iconImage;
            }
        }
        
        public override string Buffname => "공격력증가";
        public override StatType StatType => StatType.Attack;

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