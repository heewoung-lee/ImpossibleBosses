using UnityEngine;

public class Buffer_HPModifier : Duration_Buff
{
    public bool isImmediately => false;
    private Sprite _iconImage = null;
    public override Sprite BuffIconImage => _iconImage;

    public StatType statType => StatType.CurrentHp;
    public override string Buffname => "체력증가";
    public override StatType StatType => StatType.CurrentHp;
    public override void ApplyStats(BaseStats stats, float value)
    {
        stats.Plus_Current_Hp_Abillity((int)value);
    }
    public override void RemoveStats(BaseStats stats, float value)
    {
        stats.Plus_Current_Hp_Abillity(-(int)value);
    }

    public override void SetBuffIconImage(Sprite buffImageIcon)
    {
        _iconImage = buffImageIcon;
    }
}