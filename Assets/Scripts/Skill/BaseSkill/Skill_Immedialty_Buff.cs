public abstract class Skill_Immedialty_Buff : Immediately_Buff, IBaseSkill
{
    protected abstract string skillName { get; }
    public string SkillName => skillName;
    public override string Buffname => SkillName;

}