using UnityEngine;

public abstract class Skill_Duration : BaseSkill
{
    public abstract float Duration { get; }
    public abstract void RemoveStats();
    public abstract Sprite BuffIconImage { get; }
}