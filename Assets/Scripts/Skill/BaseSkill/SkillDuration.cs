using Buffer;
using UnityEngine;

namespace Skill.BaseSkill
{
    public abstract class SkillDuration : BaseSkill
    {
        public abstract float SkillDurationTime { get; }
        public abstract void RemoveStats();
        public abstract Sprite BuffIconImage { get; }
        public abstract BuffModifier BuffModifier { get; }

        public abstract string BuffIconImagePath { get; }
    }
}