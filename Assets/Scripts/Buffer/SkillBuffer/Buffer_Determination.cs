using UnityEngine;

public class Buffer_Determination : Duration_Buff
{
    public Buffer_Determination(Sprite iconImage)
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
}