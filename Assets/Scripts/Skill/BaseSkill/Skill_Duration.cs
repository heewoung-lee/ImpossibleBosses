using System;
using Buffer;
using UnityEngine;

public abstract class Skill_Duration : BaseSkill
{
    public abstract float SkillDuration { get; }
    public abstract void RemoveStats();
    public abstract Sprite BuffIconImage { get; }
    public abstract BuffModifier Buff_Modifier { get; }

    public abstract string BuffIconImagePath { get; }
}