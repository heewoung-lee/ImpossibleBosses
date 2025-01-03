using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public interface IBaseSkill
{
    public Define.PlayerClass PlayerClass { get; }
    public string SkillName { get; }

    public float CoolTime {  get; }

    public string DescriptionText { get; }

    public Sprite SkillconImage { get; }

    public void InvokeSkill();
}