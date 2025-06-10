using UnityEngine;

public class Buffer_MoveSpeedModifier : Duration_Buff
{
    public override Sprite BuffIconImage => _iconImage;

    public override string Buffname => "이동속도증가";

    public override StatType StatType => StatType.MoveSpeed;

    private Sprite _iconImage = Managers.ResourceManager.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Boots");


    public override void ApplyStats(BaseStats stats, float value)
    {
        stats.Plus_MoveSpeed_Abillity((int)value);

    }
    public override void RemoveStats(BaseStats stats, float value)
    {
        stats.Plus_MoveSpeed_Abillity(-(int)value);

    }

    public override void SetBuffIconImage(Sprite buffImageIcon)
    {
        _iconImage = buffImageIcon;
    }
}