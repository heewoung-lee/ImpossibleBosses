using UnityEngine;

public abstract class Skill_Duration : BaseSkill
{
    public abstract float SkillDuration { get; }
    public abstract void RemoveStats();
    public abstract Sprite BuffIconImage { get; }
    public abstract Buff_Modifier Buff_Modifier { get; }
}