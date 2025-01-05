using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill
{
    public abstract Define.PlayerClass PlayerClass { get; }
    public abstract string SkillName { get; }
    public abstract float CoolTime {  get; }
    public abstract string EffectDescriptionText { get; }
    public abstract string ETCDescriptionText { get; }
    public abstract Sprite SkillconImage { get; }
    public abstract float Value { get; }
    public abstract void InvokeSkill();

    public abstract Buff_Modifier Buff_Modifier { get; }
}