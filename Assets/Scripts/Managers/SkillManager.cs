using System;
using System.Collections.Generic;

public class SkillManager : IManagerInitializable
{
    Dictionary<string, IBaseSkill> _allSKillDict = new Dictionary<string, IBaseSkill>();

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
            // Activator.CreateInstance로 인스턴스 생성 _requestType은 메타데이터 이므로 인스턴스가 아님
            //따라서 Type 메타정보를 바탕으로 인스턴스를 생성해줘야함

            IBaseSkill skill = Activator.CreateInstance(type) as IBaseSkill;

            _allSKillDict.Add(skill.SkillName, skill);
        }

    }


    private void GetAllofSkill(Type type, List<Type> typeList)
    {
        if (typeof(IBaseSkill).IsAssignableFrom(type))
        {
            typeList.Add(type);
        }
    }
}