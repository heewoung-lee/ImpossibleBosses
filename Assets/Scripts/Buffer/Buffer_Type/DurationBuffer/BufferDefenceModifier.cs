using Data.DataType.ItemType.Interface;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Stats.BaseStats;
using UnityEngine;
using Zenject;

namespace Buffer.Buffer_Type.DurationBuffer
{
    public class BufferDefenceModifier : DurationBuff
    {
        
        [Inject]private IResourcesLoader _loader;

        private Sprite _iconImage;

        public override Sprite BuffIconImage
        {
            get
            {
                if (_iconImage == null)
                {
                    _iconImage = _loader.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Shield");
                }
                return _iconImage;
            }
        }
        
        public override string Buffname => "방어력증가";

        public override StatType StatType => StatType.Defence;


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