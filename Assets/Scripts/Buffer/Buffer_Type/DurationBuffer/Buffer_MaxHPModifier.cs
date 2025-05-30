using UnityEngine;

public class Buffer_MaxHPModifier : Duration_Buff
{

    private Sprite _iconImage = null;

    public override Sprite BuffIconImage => _iconImage;

    public override string Buffname => "최대체력증가";

    public override StatType StatType => StatType.MaxHP;


    public override void RemoveStats(BaseStats stats, float value)
    {
        stats.Plus_MaxHp_Abillity(-(int)value);
    }

    public override void ApplyStats(BaseStats stats, float value)
    {
        stats.Plus_MaxHp_Abillity((int)value);
    }

    public override void SetBuffIconImage(Sprite buffImageIcon)
    {
        _iconImage = buffImageIcon;
    }
}