using UnityEngine;

public class Buffer_RoarModifier : Duration_Buff
{
    public Buffer_RoarModifier(Sprite iconImage)
    {
        _iconImage = iconImage;
    }
    public override Sprite BuffIconImage => _iconImage;
    public override string Buffname => "공격력증가";
    public override StatType StatType => StatType.Attack;
    private Sprite _iconImage = null;

    public override void ApplyStats(BaseStats stats, float value)
    {
        stats.Plus_Attack_Ability((int)value);
    }
    public override void RemoveStats(BaseStats stats, float value)
    {
        stats.Plus_Attack_Ability(-(int)value);
    }
}