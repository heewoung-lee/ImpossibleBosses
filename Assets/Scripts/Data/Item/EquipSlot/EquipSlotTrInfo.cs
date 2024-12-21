using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSlotTrInfo : MonoBehaviour
{
    Transform _equipSlotR;
    Transform _equipSlotL;

    Transform _equipSlot_Helmet;
    EquipMentSlot _helmetEquipMent;

    Transform _equipSlot_Gauntlet;
    EquipMentSlot _gauntletEquipMent;

    Transform _equipSlot_Shoes;
    EquipMentSlot _shoesEquipMent;

    Transform _equipSlot_Weapon;
    EquipMentSlot _weaponEquipMent;

    Transform _equipSlot_Ring;
    EquipMentSlot _ringEquipMent;

    Transform _equipSlot_Armor;
    EquipMentSlot _armorEquipMent;


    public Transform EquipSlot_Helmet { get => _equipSlot_Helmet;}
    public EquipMentSlot HelmetEquipMent { get => _helmetEquipMent; }

    public Transform EquipSlot_Gauntlet { get => _equipSlot_Gauntlet;}
    public EquipMentSlot GauntletEquipMent{ get => _gauntletEquipMent; }

    public Transform EquipSlot_Shoes { get => _equipSlot_Shoes;}
    public EquipMentSlot ShoesEquipMent { get => _shoesEquipMent; }

    public Transform EquipSlot_Weapon { get => _equipSlot_Weapon;}
    public EquipMentSlot WeaponEquipMent { get => _weaponEquipMent; }

    public Transform EquipSlot_Ring { get => _equipSlot_Ring;}
    public EquipMentSlot RingEquipMent {get=>_ringEquipMent; }

    public Transform EquipSlot_Armor { get => _equipSlot_Armor;}
    public EquipMentSlot ArmorEquipMent { get => _armorEquipMent; }

    void Awake()
    {
        _equipSlotR = gameObject.FindChild<Transform>("EquipSlot_R");
        _equipSlotL = gameObject.FindChild<Transform>("EquipSlot_L");


        _equipSlot_Helmet = _equipSlotR.gameObject.FindChild<Transform>("EquipSlot_Helmet");
        _helmetEquipMent = _equipSlot_Helmet.GetComponentInChildren<EquipMentSlot>();

        _equipSlot_Gauntlet = _equipSlotR.gameObject.FindChild<Transform>("EquipSlot_Gauntlet");
        _gauntletEquipMent = _equipSlot_Gauntlet.GetComponentInChildren<EquipMentSlot>();

        _equipSlot_Shoes = _equipSlotR.gameObject.FindChild<Transform>("EquipSlot_Shoes");
        _shoesEquipMent = _equipSlot_Shoes.GetComponentInChildren<EquipMentSlot>();

        _equipSlot_Weapon = _equipSlotL.gameObject.FindChild<Transform>("EquipSlot_Weapon");
        _weaponEquipMent = _equipSlot_Weapon.GetComponentInChildren<EquipMentSlot>();

        _equipSlot_Ring = _equipSlotL.gameObject.FindChild<Transform>("EquipSlot_Ring");
        _ringEquipMent = _equipSlot_Ring.GetComponentInChildren<EquipMentSlot>();

        _equipSlot_Armor = _equipSlotL.gameObject.FindChild<Transform>("EquipSlot_Armor");
        _armorEquipMent = _equipSlot_Armor.GetComponentInChildren<EquipMentSlot>();
    }
}
