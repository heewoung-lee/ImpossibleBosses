using UnityEngine;

public abstract class Skill_Duration_Buff : Duration_Buff, IBaseSkill
{
    protected abstract string skillName { get; } 
    public string SkillName => skillName;
    public override string Buffname => SkillName;
}