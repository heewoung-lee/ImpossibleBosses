using UI.SubItem;
using UnityEngine;
using Util;

namespace Data.Item.EquipSlot
{
    public class EquipSlotTrInfo : MonoBehaviour
    {
        private Transform _equipSlotR;
        private Transform _equipSlotL;
        
        private Transform _equipSlotHelmet;
        private EquipMentSlot _helmetEquipMent;
        
        private Transform _equipSlotGauntlet;
        private EquipMentSlot _gauntletEquipMent;
        
        private Transform _equipSlotShoes;
        private EquipMentSlot _shoesEquipMent;
        
        private Transform _equipSlotWeapon;
        private EquipMentSlot _weaponEquipMent;
        
        private Transform _equipSlotRing;
        private EquipMentSlot _ringEquipMent;
        
        private Transform _equipSlotArmor;
        private EquipMentSlot _armorEquipMent;


        public Transform EquipSlotHelmet { get => _equipSlotHelmet;}
        public EquipMentSlot HelmetEquipMent { get => _helmetEquipMent; }

        public Transform EquipSlotGauntlet { get => _equipSlotGauntlet;}
        public EquipMentSlot GauntletEquipMent{ get => _gauntletEquipMent; }

        public Transform EquipSlotShoes { get => _equipSlotShoes;}
        public EquipMentSlot ShoesEquipMent { get => _shoesEquipMent; }

        public Transform EquipSlotWeapon { get => _equipSlotWeapon;}
        public EquipMentSlot WeaponEquipMent { get => _weaponEquipMent; }

        public Transform EquipSlotRing { get => _equipSlotRing;}
        public EquipMentSlot RingEquipMent {get=>_ringEquipMent; }

        public Transform EquipSlotArmor { get => _equipSlotArmor;}
        public EquipMentSlot ArmorEquipMent { get => _armorEquipMent; }

        void Awake()
        {
            _equipSlotR = gameObject.FindChild<Transform>("EquipSlotR");
            _equipSlotL = gameObject.FindChild<Transform>("EquipSlotL");


            _equipSlotHelmet = _equipSlotR.gameObject.FindChild<Transform>("EquipSlot_Helmet");
            _helmetEquipMent = _equipSlotHelmet.GetComponentInChildren<EquipMentSlot>();

            _equipSlotGauntlet = _equipSlotR.gameObject.FindChild<Transform>("EquipSlot_Gauntlet");
            _gauntletEquipMent = _equipSlotGauntlet.GetComponentInChildren<EquipMentSlot>();

            _equipSlotShoes = _equipSlotR.gameObject.FindChild<Transform>("EquipSlot_Shoes");
            _shoesEquipMent = _equipSlotShoes.GetComponentInChildren<EquipMentSlot>();

            _equipSlotWeapon = _equipSlotL.gameObject.FindChild<Transform>("EquipSlot_Weapon");
            _weaponEquipMent = _equipSlotWeapon.GetComponentInChildren<EquipMentSlot>();

            _equipSlotRing = _equipSlotL.gameObject.FindChild<Transform>("EquipSlot_Ring");
            _ringEquipMent = _equipSlotRing.GetComponentInChildren<EquipMentSlot>();

            _equipSlotArmor = _equipSlotL.gameObject.FindChild<Transform>("EquipSlot_Armor");
            _armorEquipMent = _equipSlotArmor.GetComponentInChildren<EquipMentSlot>();
        }
    }
}
