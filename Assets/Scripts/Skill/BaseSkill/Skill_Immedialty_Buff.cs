using UnityEngine;

public abstract class Skill_Immedialty_Buff : Immediately_Buff, IBaseSkill
{
    protected abstract string skillName { get; }
    public string SkillName => skillName;
    public override string Buffname => SkillName;
    public abstract float CoolTime { get; }
    public abstract Define.PlayerClass PlayerClass { get; }
    public abstract string DescriptionText { get; }
    public abstract Sprite SkillconImage { get; }

    public abstract void InvokeSkill();
}