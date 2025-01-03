using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public abstract class BaseSkill
{
    public abstract Define.PlayerClass PlayerClass { get; }
    public abstract string SkillName { get; }
    public abstract float CoolTime {  get; }
    public abstract string DescriptionText { get; }
    public abstract Sprite SkillconImage { get; }
    public abstract void InvokeSkill();
}