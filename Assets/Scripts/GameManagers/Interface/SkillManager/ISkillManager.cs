using System;
using System.Collections.Generic;
using Skill.BaseSkill;
using UI.Scene.SceneUI;

namespace GameManagers.Interface.SkillManager
{
    public interface ISkillManager
    {
        public IDictionary<string,BaseSkill> GetSkills();
        public event Action DoneUISkilBarInitEvent;
        public UISkillBar GetUISkillBar();
        public void Invoke_Done_UI_SKilBar_Init_Event();
    }
}
