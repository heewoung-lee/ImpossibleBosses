using UnityEngine;

public class Skill_Buff_Roar : Skill_Duration_Buff
{
    public override Sprite IconImage => Managers.ResourceManager.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/Icon_PictoIcons/128/function_icon_powerup");

    public override StatType StatType => StatType.Defence;

    public override float CoolTime => throw new System.NotImplementedException();

    protected override string skillName => "절대방어";

    public override void ApplyStats(BaseStats stats, float value)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveStats(BaseStats stats, float value)
    {
        throw new System.NotImplementedException();
    }
}