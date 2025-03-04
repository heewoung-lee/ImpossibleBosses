using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : IManagerInitializable
{
    Dictionary<string, BaseSkill> _allSKillDict = new Dictionary<string, BaseSkill>();
    public Dictionary<string, BaseSkill> AllSKillDict { get => _allSKillDict; }

    public Action Done_UI_SKilBar_Init_Event;

    List<Type> _skillType = new List<Type>();

    private UI_SkillBar _ui_SkillBar;
    public UI_SkillBar UI_SkillBar
    {
        get
        {
            if (_ui_SkillBar == null)
            {
                if(Managers.UI_Manager.Try_Get_Scene_UI(out UI_SkillBar skillbar))
                {
                    _ui_SkillBar = skillbar;
                }
            }
            return _ui_SkillBar;
        }
    }
    public void Init()
    {
        //Skill/AllofSkill에 있는 타입들을 가져온다.
        _skillType = Managers.DataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Skill/AllofSkills", GetAllofSkill);
        foreach (Type type in _skillType)
        {
           
            BaseSkill skill = Activator.CreateInstance(type) as BaseSkill;

            AllSKillDict.Add(skill.SkillName, skill);
        }

    }


    private void GetAllofSkill(Type type, List<Type> typeList)
    {
        if (typeof(BaseSkill).IsAssignableFrom(type))
        {
            typeList.Add(type);
        }
    }
}