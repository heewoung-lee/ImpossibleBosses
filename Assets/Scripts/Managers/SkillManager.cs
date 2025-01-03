using System;
using System.Collections.Generic;

public class SkillManager : IManagerInitializable
{
    Dictionary<string, BaseSkill> _allSKillDict = new Dictionary<string, BaseSkill>();
    public Dictionary<string, BaseSkill> AllSKillDict { get => _allSKillDict; }

    List<Type> _skillType = new List<Type>();

    private UI_SkillBar _ui_SkillBar;
    public UI_SkillBar UI_SkillBar
    {
        get
        {
            if (_ui_SkillBar == null)
                _ui_SkillBar = Managers.UI_Manager.Get_Scene_UI<UI_SkillBar>();

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